using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Yahoo.Streamer;

namespace MarketFeedService.Infrastructure.Messaging.Adapters;

public class YahooFinance : IMarketDataFeedAdapter
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<YahooFinance> _logger;

    public YahooFinance(IConfiguration configuration, ILogger<YahooFinance> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async IAsyncEnumerable<StockTradeMessage> StreamAsync(IEnumerable<string> symbols, [EnumeratorCancellation] CancellationToken ct)
    {
        using ClientWebSocket ws = new();
        await ws.ConnectAsync(new Uri(_configuration["YahooFinance:WsUrl"]!), ct);

        // Subscribe
        var subscribePayload = JsonSerializer.Serialize(new
        {
            subscribe = symbols.ToArray()
        });
        _logger.LogInformation("Subscribe Message Payload: {SubscribeMessagePayload}", subscribePayload);
        await SendMessageAsync(ws, subscribePayload, ct);

        while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var text = await ReceiveMessageAsync(ws, ct);
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            foreach (var line in text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                PricingData? pd;
                try
                {
                    var bytes = Convert.FromBase64String(line);
                    pd = PricingData.Parser.ParseFrom(bytes);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to decode Yahoo Finance Frame: {Frame}", line);
                    continue;
                }

                if (pd.QuoteType == PricingData.Types.QuoteType.Heartbeat)
                {
                    continue;
                }

                _logger.LogInformation("Decoded Yahoo Finance message: {Message}", pd);

                yield return new StockTradeMessage
                {
                    StockTradeId = 0,
                    Symbol = pd.Id,
                    ExchangeCode = pd.Exchange,
                    Price = (decimal)pd.Price,
                    Size = pd.LastSize,
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(pd.Time),
                };
            }

        }
    }

    private static async Task SendMessageAsync(ClientWebSocket ws, string message, CancellationToken ct)
    {
        await ws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, ct);
    }

    private static async Task<string> ReceiveMessageAsync(ClientWebSocket ws, CancellationToken ct)
    {
        byte[] buffer = new byte[1024 * 4];
        var sb = new StringBuilder();

        WebSocketReceiveResult result;
        do
        {
            result = await ws.ReceiveAsync(buffer, ct);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", ct);
                return string.Empty;
            }
            sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        }
        while (!result.EndOfMessage);

        return sb.ToString();
    }
}
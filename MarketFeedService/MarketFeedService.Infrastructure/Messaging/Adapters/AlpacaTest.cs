using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using MarketFeedService.Core.Entities.ApiEntities;
using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketFeedService.Infrastructure.Messaging.Adapters;

public class AlpacaTest : IMarketDataFeedAdapter
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlpacaTest> _logger;

    public AlpacaTest(IConfiguration configuration, ILogger<AlpacaTest> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // TODO: add error handling and retry connection
    public async IAsyncEnumerable<StockTradeMessage> StreamAsync(IEnumerable<string> symbols, [EnumeratorCancellation] CancellationToken ct)
    {
        using ClientWebSocket ws = new();
        await ws.ConnectAsync(new Uri(_configuration["AlpacaMarket:StockWsUrlTest"]!), ct);

        // Wait for "connected" message
        string connectedMsg = await ReceiveMessageAsync(ws, ct);
        if (!connectedMsg.Contains("connected"))
        {
            throw new InvalidOperationException($"Failed to connect to AlpacaMarket: {connectedMsg}");
        }
        _logger.LogInformation("AlpacaMarket WebSocket connected");


        // See the docs for JSON payload structures
        // https://docs.alpaca.markets/docs/streaming-market-data#authentication

        // Authenticate
        var authPayload = JsonSerializer.Serialize(new
        {
            action = "auth",
            key = _configuration["AlpacaMarket:ApiKey"],
            secret = _configuration["AlpacaMarket:SecretKey"]
        });
        await SendMessageAsync(ws, authPayload, ct);

        string authResponse = await ReceiveMessageAsync(ws, ct);
        var authResponses = JsonSerializer.Deserialize<List<AlpacaMarketAuthResponse>>(authResponse);

        if (authResponses?.All(r => r.Msg != "authenticated") ?? true)
        {
            throw new UnauthorizedAccessException($"Authentication failed: {authResponse}");
        }
        _logger.LogInformation("Authentication successful");

        // Subscribe
        var subscribePayload = JsonSerializer.Serialize(new
        {
            action = "subscribe",
            trades = symbols.ToArray(),
        });
        await SendMessageAsync(ws, subscribePayload, ct);
        string subscribeResponse = await ReceiveMessageAsync(ws, ct);
        _logger.LogInformation("Subscribe response: {SubscribeResponse}", subscribeResponse);

        // Stream messages
        while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var update = await ReceiveMessageAsync(ws, ct);
            _logger.LogInformation("Trade Json: {TradeJson}", update);
            var updates = JsonSerializer.Deserialize<List<AlpacaMarketTradeResponse>>(update);

            foreach (var trade in updates ?? Enumerable.Empty<AlpacaMarketTradeResponse>())
            {
                yield return new StockTradeMessage
                {
                    StockTradeId = trade.TradeId,
                    ExchangeCode = trade.ExchangeCode,
                    Symbol = trade.Symbol,
                    Size = trade.Size,
                    Price = trade.Price,
                    Timestamp = trade.Timestamp
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
        var result = await ws.ReceiveAsync(buffer, ct);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }

}
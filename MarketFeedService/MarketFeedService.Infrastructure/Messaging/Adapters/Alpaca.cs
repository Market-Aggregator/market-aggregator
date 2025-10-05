using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using MarketFeedService.Core.Entities.ApiEntities;
using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Entities.Enums;
using MarketFeedService.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketFeedService.Infrastructure.Messaging.Adapters;

public sealed class Alpaca : IMarketDataFeedAdapter, IAsyncDisposable
{
    private readonly ClientWebSocket _socket = new();
    private readonly IConfiguration _configuration;
    private readonly ILogger<Alpaca> _logger;

    public Alpaca(IConfiguration configuration, ILogger<Alpaca> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // TODO: add error handling and retry connection
    public async IAsyncEnumerable<MarketEvent> StreamAsync([EnumeratorCancellation] CancellationToken ct)
    {
        // See the docs for JSON payload structures
        // https://docs.alpaca.markets/docs/streaming-market-data#authentication

        // Stream messages
        while (_socket.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var alpacaMessage = await ReceiveAsync(ct);

            var jsonDoc = JsonDocument.Parse(alpacaMessage);

            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var elem in jsonDoc.RootElement.EnumerateArray())
                {
                    _logger.LogInformation("Json element {jsonElem}", elem);
                    if (!elem.TryGetProperty("T", out var typeProperty))
                    {
                        _logger.LogWarning("Message missing T property: {Message}", elem.GetRawText());
                        continue;
                    }

                    var type = typeProperty.ToString();

                    switch (type)
                    {
                        case "t":
                            var trade = elem.Deserialize<AlpacaMarketTradeResponse>();
                            if (trade is not null)
                            {
                                yield return new StockTradeMessage(
                                        Symbol: trade.Symbol,
                                        Timestamp: trade.Timestamp,
                                        Event: MarketEvents.Trade,
                                        StockTradeId: trade.TradeId,
                                        ExchangeCode: trade.ExchangeCode,
                                        Price: trade.Price,
                                        Size: trade.Size,
                                        Currency: Currency.USD
                                        );
                            }
                            break;

                        case "q":
                            var quote = elem.Deserialize<AlpacaMarketQuoteResponse>();
                            if (quote is not null)
                            {
                                yield return new StockQuoteMessage(
                                        Symbol: quote.Symbol,
                                        Timestamp: quote.Timestamp,
                                        Event: MarketEvents.Quote,
                                        AskExchangeCode: quote.AskExchangeCode,
                                        AskPrice: quote.AskPrice,
                                        AskSize: quote.AskSize,
                                        BidExchangeCode: quote.BidExchangeCode,
                                        BidPrice: quote.BidPrice,
                                        BidSize: quote.BidSize,
                                        Currency: Currency.USD
                                        );
                            }
                            break;
                    }
                }
            }
        }
    }

    // abstract out into helper functions
    private async Task SendAsync(object payload, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload);
        await _socket.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, ct);
    }

    private async Task<string> ReceiveAsync(CancellationToken ct)
    {
        byte[] buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;
        var sb = new StringBuilder();

        do
        {
            result = await _socket.ReceiveAsync(buffer, ct);
            sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        return sb.ToString();
    }

    public async Task ConnectAsync(CancellationToken ct)
    {
        await _socket.ConnectAsync(new Uri(_configuration["AlpacaMarket:StockWsUrl"]!), ct);
        string connectedMsg = await ReceiveAsync(ct);
        if (!connectedMsg.Contains("connected"))
        {
            throw new InvalidOperationException($"Failed to connect to AlpacaMarket: {connectedMsg}");
        }
        _logger.LogInformation("AlpacaMarket WebSocket connected");
    }

    public async Task AuthenticateAsync(CancellationToken ct)
    {
        await SendAsync(new
        {
            action = "auth",
            key = _configuration["AlpacaMarket:ApiKey"],
            secret = _configuration["AlpacaMarket:SecretKey"]
        }, ct);

        string authResponse = await ReceiveAsync(ct);
        var authResponses = JsonSerializer.Deserialize<List<AlpacaMarketAuthResponse>>(authResponse);

        if (authResponses?.All(r => r.Msg != "authenticated") ?? true)
        {
            throw new UnauthorizedAccessException($"Authentication failed: {authResponse}");
        }
        _logger.LogInformation("Auth Response: {AuthResponse}", authResponses[0]);
    }

    public async Task SubscribeAsync(IEnumerable<string> symbols, MarketFeeds feeds, CancellationToken ct)
    {
        var trades = (feeds & MarketFeeds.Trades) != 0 ? symbols.ToArray() : Array.Empty<string>();
        var quotes = (feeds & MarketFeeds.Quotes) != 0 ? symbols.ToArray() : Array.Empty<string>();
        await SendAsync(new { action = "subscribe", trades, quotes }, ct);

        string subscribeResponse = await ReceiveAsync(ct);
        _logger.LogInformation("Subscribe response: {SubscribeResponse}", subscribeResponse);
    }

    public async Task CloseAsync(CancellationToken ct)
    {
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket Normal Closure", ct);
    }

    public ValueTask DisposeAsync()
    {
        _socket.Dispose();
        return ValueTask.CompletedTask;
    }
}
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using MarketFeedService.Core.Entities.ApiEntities;
using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketFeedService.Infrastructure.Messaging.Adapters;

public class AlpacaWebSocketClient : ILiveMarketDataClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlpacaWebSocketClient> _logger;
    private readonly IStockTradeProducer _producer;

    public AlpacaWebSocketClient(IStockTradeProducer producer, IConfiguration configuration, ILogger<AlpacaWebSocketClient> logger)
    {
        _producer = producer;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ConnectAndStreamAsync(IEnumerable<string> symbols, CancellationToken ct)
    {
        // test stream endpoint available outside market hours
        Uri uri = new(_configuration["AlpacaMarket:StockWsUrlTest"]!);

        using ClientWebSocket ws = new();
        await ws.ConnectAsync(uri, ct);
        _logger.LogInformation("AlpacaMarket WebSocket connected");

        string connectedMsg = await ReceiveMessageAsync(ws, ct);
        if (!connectedMsg.Contains("connected"))
        {
            _logger.LogError("Failed to connect: {Message}", connectedMsg);
            return;
        }
        _logger.LogInformation("AlpacaMarket WebSocket awaiting authentication");


        // See the docs for JSON payload structures
        // https://docs.alpaca.markets/docs/streaming-market-data#authentication

        var authPayload = JsonSerializer.Serialize(new
        {
            action = "auth",
            key = _configuration["AlpacaMarket:ApiKey"],
            secret = _configuration["AlpacaMarket:SecretKey"]
        });
        await SendMessageAsync(ws, authPayload, ct);
        _logger.LogInformation("Auth message sent.");

        string authResponse = await ReceiveMessageAsync(ws, ct);

        // TODO: temp - remove later
        string[] requiredAuth = ["success", "authenticated"];
        if (!requiredAuth.All(authResponse.Contains))
        {
            _logger.LogError("Authentication Failed {Message}", authResponse);
            // TODO: maybe throw exception instead
            return;
        }
        _logger.LogInformation("Auth Success: {AuthMessage}", authResponse);

        var subscribePayload = JsonSerializer.Serialize(new
        {
            action = "subscribe",
            trades = symbols.ToArray(),
        });
        await SendMessageAsync(ws, subscribePayload, ct);
        _logger.LogInformation("Subscribe message sent.");

        string subscribeResponse = await ReceiveMessageAsync(ws, ct);
        _logger.LogInformation("Subscribe response: {SubscribeResponse}", subscribeResponse);

        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            var update = await ReceiveMessageAsync(ws, ct);
            _logger.LogInformation("Trade Update Json: {UpdateJson}", update);

            var updates = JsonSerializer.Deserialize<List<TradeResponse>>(update);

            foreach (TradeResponse trade in updates)
            {
                var stockTradeEntity = new StockTrade
                {
                    StockTradeId = trade.TradeId,
                    Exchange = trade.ExchangeCode,
                    Symbol = trade.Symbol,
                    Size = trade.Size,
                    Price = trade.Price,
                    Timestamp = trade.Timestamp
                };

                // TODO: use confluent kafka serializer on ProducerBuilder using kafka schema registry
                // serialize using .net api temporarily
                var tradeJson = JsonSerializer.Serialize(stockTradeEntity);
                string topic = $"{trade.ExchangeCode}.{trade.Symbol}";

                await _producer.ProduceAsync(topic, trade.Symbol, tradeJson, ct);
                _logger.LogInformation("Stock Trade Event published to Kafka Topic: {Topic}", topic);
            }

            _producer.Flush(ct);
        }
    }

    private static async Task SendMessageAsync(ClientWebSocket ws, string message, CancellationToken ct)
    {
        await ws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, ct);
    }

    // TODO: make this return a strongly-typed response based on AlpacaMarket's message format
    private static async Task<string> ReceiveMessageAsync(ClientWebSocket ws, CancellationToken ct)
    {
        // TODO: revisit - buffer might not have to be this big
        byte[] buffer = new byte[8192];
        var result = await ws.ReceiveAsync(buffer, ct);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }

}
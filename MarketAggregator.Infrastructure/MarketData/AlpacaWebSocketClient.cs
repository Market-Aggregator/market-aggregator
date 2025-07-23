using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using MarketAggregator.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketAggregator.Infrastructure.MarketData;

public class AlpacaWebSocketClient : ILiveMarketDataClient
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AlpacaWebSocketClient> _logger;

    public AlpacaWebSocketClient(IConfiguration configuration, ILogger<AlpacaWebSocketClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ConnectAndStreamAsync(IEnumerable<string> symbols, CancellationToken ct)
    {
        // Uri uri = new("wss://stream.data.alpaca.markets/v2/iex");

        // test stream endpoint available outside market hours
        Uri uri = new("wss://stream.data.alpaca.markets/v2/test");

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
        _logger.LogInformation("Sent auth message.");

        string authResponse = await ReceiveMessageAsync(ws, ct);

        // TODO: temp - remove later
        string[] requiredAuth = ["success", "authenticated"];
        if (!requiredAuth.All(kw => authResponse.Contains(kw)))
        {
            _logger.LogError("Authentication Failed {Message}", authResponse);
            // TODO: maybe throw exception instead
            return;
        }

        var subscribePayload = JsonSerializer.Serialize(new
        {
            action = "subscribe",
            // trades = symbols.ToArray(),

            // test symbol to use with test stream endpoint
            trades = "[\"FAKEPACCA\"]",
        });
        await SendMessageAsync(ws, subscribePayload, ct);

        byte[] buffer = new byte[8192];
        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {

            // TODO: read ws response, transform to entity, and publish to event stream
        }
    }

    private static async Task SendMessageAsync(ClientWebSocket ws, string message, CancellationToken ct)
    {
        ArraySegment<byte> bytes = new(Encoding.UTF8.GetBytes(message));
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
    }

    // TODO: make this return a strongly-typed response based on AlpacaMarket's message format
    private static async Task<string> ReceiveMessageAsync(ClientWebSocket ws, CancellationToken ct)
    {
        byte[] buffer = new byte[8192];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }

}
using System.Text.Json;

using MarketFeedService.Core.Interfaces;

namespace MarketFeedService.Worker.Workers;

public class AlpacaMarketWorker : BackgroundService
{
    private readonly IMarketDataFeedAdapter _marketClient;
    private readonly IStockTradeProducer _producer;
    private readonly ILogger<AlpacaMarketWorker> _logger;
    // private static readonly string[] Symbols = ["APPL", "MSFT", "GOOG"];
    private static readonly string[] Symbols = ["FAKEPACA"];

    public AlpacaMarketWorker(IMarketDataFeedAdapter marketDataFeedAdapter, IStockTradeProducer producer, ILogger<AlpacaMarketWorker> logger)
    {
        _marketClient = marketDataFeedAdapter;
        _producer = producer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        await foreach (var trade in _marketClient.StreamAsync(Symbols, stoppingToken))
        {
            var tradeJson = JsonSerializer.Serialize(trade);
            var topic = $"{trade.Exchange}.{trade.Symbol}";

            await _producer.ProduceAsync(topic, trade.Symbol, tradeJson, stoppingToken);
            _logger.LogInformation("Produced trade ({Trade}) to Kafka Topic: {Topic}", tradeJson, topic);
        }

        _producer.Flush(stoppingToken);
    }
}
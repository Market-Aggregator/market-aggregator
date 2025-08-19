using System.Text.Json;

using MarketFeedService.Core.Interfaces;

namespace MarketFeedService.Worker.Workers;

public class AlpacaTestMarketWorker : BackgroundService
{
    private readonly IMarketDataFeedAdapter _marketClient;
    private readonly IStockTradeProducer _producer;
    private readonly ILogger<AlpacaMarketWorker> _logger;
    private static readonly string[] Symbols = ["FAKEPACA"];

    public AlpacaTestMarketWorker([FromKeyedServices("alpacatest")] IMarketDataFeedAdapter marketDataFeedAdapter, IStockTradeProducer producer, ILogger<AlpacaMarketWorker> logger)
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

            // One topic per exchange, using Symbol as the Key ensures same symbol goes to same partition within the topic
            await _producer.ProduceAsync(trade.ExchangeCode, trade.Symbol, tradeJson, stoppingToken);
        }

        _producer.Flush(stoppingToken);
    }
}
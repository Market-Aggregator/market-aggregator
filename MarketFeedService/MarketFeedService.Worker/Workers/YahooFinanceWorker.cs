using System.Text.Json;

using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Entities.Enums;
using MarketFeedService.Core.Interfaces;

namespace MarketFeedService.Worker.Workers;

public class YahooFinanceWorker : BackgroundService
{
    private readonly IMarketDataFeedAdapter _marketClient;
    private readonly IStockTradeProducer _producer;
    private readonly ILogger<YahooFinanceWorker> _logger;
    private static readonly string[] Symbols = ["NVDA"];

    public YahooFinanceWorker([FromKeyedServices("yahoofinance")] IMarketDataFeedAdapter marketDataFeedAdapter, IStockTradeProducer producer, ILogger<YahooFinanceWorker> logger)
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

        await foreach (var ev in _marketClient.StreamAsync(stoppingToken))
        {
            var eventJson = JsonSerializer.Serialize(ev);

            // One topic per exchange, using Symbol as the Key ensures same symbol goes to same partition within the topic
            switch (ev)
            {
                case StockTradeMessage trade:
                    string topic = $"{MarketEvents.Trade}.{trade.ExchangeCode}";
                    await _producer.ProduceAsync(topic, trade.Symbol, eventJson, stoppingToken);
                    _logger.LogInformation("Produced trade event to Kafka topic: {Topic}", trade.ExchangeCode);
                    break;
                case StockQuoteMessage quote:
                    await _producer.ProduceAsync(MarketEvents.Quote.ToString(), quote.Symbol, eventJson, stoppingToken);
                    _logger.LogInformation("Produced quote event to Kafka topic: {Topic}", MarketEvents.Quote);
                    break;
            }
        }

        _producer.Flush(stoppingToken);
    }
}
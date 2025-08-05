using MarketOverviewService.Core.Interfaces;

namespace MarketOverviewService.Api.Workers;

public class AlpacaMarketConsumerWorker : BackgroundService
{
    private readonly IPublisher _publisher;
    private readonly ILogger<AlpacaMarketConsumerWorker> _logger;
    private readonly IMarketDataConsumer _marketDataConsumer;

    public AlpacaMarketConsumerWorker(IPublisher publisher, IMarketDataConsumer marketDataConsumer, ILogger<AlpacaMarketConsumerWorker> logger)
    {
        _publisher = publisher;
        _marketDataConsumer = marketDataConsumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("AlpacaMarket Consumer Worker running at: {time}", DateTimeOffset.Now);
        }

        await foreach (var trade in _marketDataConsumer.ConsumeAsync("N", stoppingToken))
        {
            await _publisher.BroadcastTradeAsync(trade);
        }
    }
}
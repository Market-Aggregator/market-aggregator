using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure;

namespace MarketOverviewService.Worker.Workers;

public class AlpacaMarketConsumerWorker : BackgroundService
{
    private readonly IPublisher _publisher;
    private readonly ILogger<AlpacaMarketConsumerWorker> _logger;
    private readonly ILiveMarketDataConsumer _marketDataConsumer;

    public AlpacaMarketConsumerWorker(IPublisher publisher, ILiveMarketDataConsumer marketDataConsumer, ILogger<AlpacaMarketConsumerWorker> logger)
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

        while (!stoppingToken.IsCancellationRequested)
        {
            var stockTrade = await _marketDataConsumer.ConsumeAsync(stoppingToken);
            if (stockTrade is not null) {
                await _publisher.BroadcastTradeAsync(stockTrade);
            }
        }
    }
}
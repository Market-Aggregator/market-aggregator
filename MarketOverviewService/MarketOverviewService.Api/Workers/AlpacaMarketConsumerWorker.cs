using MarketOverviewService.Core.Interfaces;

namespace MarketOverviewService.Api.Workers;

public class AlpacaMarketConsumerWorker : BackgroundService
{
    private readonly IPublisher _publisher;
    private readonly ILogger<AlpacaMarketConsumerWorker> _logger;
    private readonly IMarketDataConsumer _marketDataConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AlpacaMarketConsumerWorker(
            IPublisher publisher,
            IMarketDataConsumer marketDataConsumer,
            ILogger<AlpacaMarketConsumerWorker> logger,
            IServiceScopeFactory serviceScopeFactory
            )
    {
        _publisher = publisher;
        _marketDataConsumer = marketDataConsumer;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("AlpacaMarket Consumer Worker running at: {time}", DateTimeOffset.Now);
        }

        await foreach (var trade in _marketDataConsumer.ConsumeAsync("V", stoppingToken))
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IStockTradeRepository stockTradeRepo = scope.ServiceProvider.GetRequiredService<IStockTradeRepository>();

            await _publisher.BroadcastTradeAsync(trade);

            // TODO: enqueue to in-memory queue (using channels) to persist asynchronously
            // so that we don't block streaming of trades to clients
            await stockTradeRepo.CreateAsync(trade);
        }
    }
}
using MarketOverviewService.Core.Interfaces;

namespace MarketOverviewService.Api.Workers;

public class AlpacaMarketConsumerWorker : BackgroundService
{
    private readonly IPublisher _publisher;
    private readonly ILogger<AlpacaMarketConsumerWorker> _logger;
    private readonly IMarketDataConsumer _marketDataConsumer;
    private readonly IStockTradeRepository _stockTradeRepo;

    public AlpacaMarketConsumerWorker(
            IPublisher publisher,
            IMarketDataConsumer marketDataConsumer,
            ILogger<AlpacaMarketConsumerWorker> logger,
            IStockTradeRepository stockTradeRepo
            )
    {
        _publisher = publisher;
        _marketDataConsumer = marketDataConsumer;
        _logger = logger;
        _stockTradeRepo = stockTradeRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("AlpacaMarket Consumer Worker running at: {time}", DateTimeOffset.Now);
        }

        await foreach (var trade in _marketDataConsumer.ConsumeAsync("V", stoppingToken))
        {
            await _publisher.BroadcastTradeAsync(trade);

            // TODO: enqueue to in-memory queue (using channels) to persist asynchronously
            // so that we don't block streaming of trades to clients
            await _stockTradeRepo.CreateAsync(trade);
        }
    }
}
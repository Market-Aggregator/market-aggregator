using MarketAggregator.Core.Interfaces;

namespace MarketAggregator.Ingestor.WorkerService;

public class StreamMarketDataWorker : BackgroundService
{
    private readonly ILiveMarketDataClient _marketDataClient;
    private readonly ILogger<StreamMarketDataWorker> _logger;
    // private static readonly string[] Symbols = ["APPL", "MSFT", "GOOG"];
    private static readonly string[] Symbols = ["FAKEPACA"];

    public StreamMarketDataWorker(ILiveMarketDataClient marketDataClient, ILogger<StreamMarketDataWorker> logger)
    {
        _marketDataClient = marketDataClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        await _marketDataClient.ConnectAndStreamAsync(Symbols, stoppingToken);
    }
}
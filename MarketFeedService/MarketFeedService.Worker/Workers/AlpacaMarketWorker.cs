
using MarketFeedService.Core.Interfaces;

namespace MarketFeedService.Worker.Workers;

public class AlpacaMarketWorker : BackgroundService
{
    private readonly ILiveMarketDataClient _marketDataClient;
    private readonly ILogger<AlpacaMarketWorker> _logger;
    // private static readonly string[] Symbols = ["APPL", "MSFT", "GOOG"];
    private static readonly string[] Symbols = ["FAKEPACA"];

    public AlpacaMarketWorker(ILiveMarketDataClient marketDataClient, ILogger<AlpacaMarketWorker> logger)
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
using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IMarketDataConsumer
{
    IAsyncEnumerable<MarketEvent> ConsumeAsync(string exchange, CancellationToken ct);
}
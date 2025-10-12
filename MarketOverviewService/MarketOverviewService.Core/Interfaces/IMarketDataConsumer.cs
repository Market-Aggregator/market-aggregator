using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IMarketDataConsumer
{
    IAsyncEnumerable<MarketEvent> ConsumeAsync(IEnumerable<string> topics, CancellationToken ct);
}
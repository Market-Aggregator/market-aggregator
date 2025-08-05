using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IMarketDataConsumer
{
    IAsyncEnumerable<StockTrade> ConsumeAsync(string exchange, CancellationToken ct);
}
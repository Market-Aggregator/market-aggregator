using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IMarketDataConsumer
{
    IAsyncEnumerable<StockTradeMessage> ConsumeAsync(string exchange, CancellationToken ct);
}
using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface ILiveMarketDataConsumer {
    Task<StockTrade?> ConsumeAsync(CancellationToken ct);
}
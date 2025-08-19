using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IPublisher {
    Task BroadcastTradeAsync(StockTradeMessage trade);
}
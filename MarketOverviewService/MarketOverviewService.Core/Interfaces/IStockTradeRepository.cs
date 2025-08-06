using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IStockTradeRepository {
    Task<StockTrade?> CreateAsync(StockTrade stockTrade);
}
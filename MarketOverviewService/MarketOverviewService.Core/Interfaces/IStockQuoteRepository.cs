using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Core.Interfaces;

public interface IStockQuoteRepository {
    Task<StockQuote?> CreateAsync(StockQuote stockQuote);
}
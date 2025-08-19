using MarketFeedService.Core.Entities.DataEntities;

namespace MarketFeedService.Core.Interfaces;

public interface IMarketDataFeedAdapter
{
    IAsyncEnumerable<StockTradeMessage> StreamAsync(IEnumerable<string> symbols, CancellationToken ct);
}
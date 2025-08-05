using MarketFeedService.Core.Entities.DataEntities;

namespace MarketFeedService.Core.Interfaces;

public interface IMarketDataFeedAdapter
{
    // TODO: add the below params
    IAsyncEnumerable<StockTrade> StreamAsync(IEnumerable<string> symbols, CancellationToken ct);
}
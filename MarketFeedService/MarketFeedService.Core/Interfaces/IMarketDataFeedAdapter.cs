using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Entities.Enums;

namespace MarketFeedService.Core.Interfaces;

public interface IMarketDataFeedAdapter
{
    IAsyncEnumerable<MarketEvent> StreamAsync(
            IEnumerable<string> symbols,
            MarketFeeds feeds,
            CancellationToken ct);
}
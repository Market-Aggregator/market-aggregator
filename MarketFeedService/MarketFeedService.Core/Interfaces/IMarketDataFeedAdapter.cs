using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Entities.Enums;

namespace MarketFeedService.Core.Interfaces;

public interface IMarketDataFeedAdapter
{
    Task ConnectAsync(CancellationToken ct);
    Task AuthenticateAsync(CancellationToken ct);
    Task SubscribeAsync(
            IEnumerable<string> symbols,
            MarketFeeds feeds,
            CancellationToken ct);

    IAsyncEnumerable<MarketEvent> StreamAsync(CancellationToken ct);

    Task CloseAsync(CancellationToken ct);
}

using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Interfaces;

namespace MarketFeedService.Infrastructure.Messaging.Adapters;

public class Finnhub : IMarketDataFeedAdapter
{
    public IAsyncEnumerable<StockTrade> StreamAsync(IEnumerable<string> symbols, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
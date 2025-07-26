namespace MarketFeedService.Core.Interfaces;

public interface ILiveMarketDataClient {
    // TODO: add the below params
    Task ConnectAndStreamAsync(IEnumerable<string> symbols, CancellationToken ct);
}
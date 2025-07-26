namespace MarketFeedService.Core.Interfaces;

public interface IStockTradeProducer
{
    public Task ProduceAsync(string topic, string key, string payload, CancellationToken ct);
    public void Flush(CancellationToken ct);
}
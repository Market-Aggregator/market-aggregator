namespace MarketFeedService.Core.Entities.DataEntities;

public record StockTradeMessage
{
    public long StockTradeId { get; set; }
    public required string Symbol { get; set; }
    public required string ExchangeCode { get; set; }
    public decimal Price { get; set; }
    public long Size { get; set; }
    public DateTimeOffset Timestamp { get; set; }
};
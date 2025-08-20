using MarketFeedService.Core.Entities.Enums;

namespace MarketFeedService.Core.Entites.DataEntities;

public record StockQuoteMessage
{
    public required string Symbol { get; set; }
    public required string AskExchangeCode { get; set; }
    public decimal AskPrice { get; set; }
    public long AskSize { get; set; }
    public required string BidExchangeCode { get; set; }
    public decimal BidPrice { get; set; }
    public long BidSize { get; set; }
    public long Size { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Currency? Currency { get; set; }
}
namespace MarketOverviewService.Core.Entities;

public record StockTradeMessage(
     long StockTradeId,
     string Symbol,
     string ExchangeCode,
     decimal Price,
     long Size,
     DateTimeOffset Timestamp);
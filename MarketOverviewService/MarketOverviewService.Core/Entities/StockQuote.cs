using MarketOverviewService.Core.Entities.Enums;

namespace MarketOverviewService.Core.Entities;

public record StockQuote(
        string Symbol,
        DateTimeOffset Timestamp,
        decimal AskPrice,
        long AskSize,
        string BidExchangeCode,
        decimal BidPrice,
        long BidSize,
        Currency Currency);
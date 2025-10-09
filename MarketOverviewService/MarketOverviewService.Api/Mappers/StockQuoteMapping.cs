using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Api.Mappers;

public static class StockQuoteMapping
{
    public static StockQuote ToEntity(this StockQuoteMessage stockQuoteMessage)
    {
        return new StockQuote
        {
            Id = Guid.NewGuid(),
            Symbol = stockQuoteMessage.Symbol,
            Timestamp = stockQuoteMessage.Timestamp,
            AskPrice = stockQuoteMessage.AskPrice,
            AskSize = stockQuoteMessage.AskSize,
            BidExchangeCode = stockQuoteMessage.BidExchangeCode,
            BidPrice = stockQuoteMessage.BidPrice,
            BidSize = stockQuoteMessage.BidSize,
            Currency = stockQuoteMessage.Currency
        };
    }
}
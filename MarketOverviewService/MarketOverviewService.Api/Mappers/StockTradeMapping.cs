using MarketOverviewService.Core.Entities;

namespace MarketOverviewService.Api.Mappers;

public static class StockTradeMapping
{
    public static StockTrade ToEntity(this StockTradeMessage stockTradeMessage)
    {
        return new StockTrade
        {
            // StockTradeId = stockTradeMessage.StockTradeId,
            StockTradeId = Guid.NewGuid(),
            Symbol = stockTradeMessage.Symbol,
            ExchangeCode = stockTradeMessage.ExchangeCode,
            Price = stockTradeMessage.Price,
            Size = stockTradeMessage.Size,
            Timestamp = stockTradeMessage.Timestamp,
        };
    }
}
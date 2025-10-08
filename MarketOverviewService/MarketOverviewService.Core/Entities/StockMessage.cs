using MarketOverviewService.Core.Entities.Enums;

namespace MarketOverviewService.Core.Entities;

// TODO: this should be abstracted into a shared library for the microservices
public abstract record MarketEvent(
        string Symbol,
        DateTimeOffset Timestamp,
        MarketEvents Event
        );

public sealed record StockTradeMessage(
        string Symbol,
        DateTimeOffset Timestamp,
        MarketEvents Event,
        long StockTradeId,
        string ExchangeCode,
        decimal Price,
        long Size,
        Currency Currency
        ) : MarketEvent(Symbol, Timestamp, Event);

public sealed record StockQuoteMessage(
        string Symbol,
        DateTimeOffset Timestamp,
        MarketEvents Event,
        string AskExchangeCode,
        decimal AskPrice,
        long AskSize,
        string BidExchangeCode,
        decimal BidPrice,
        long BidSize,
        Currency Currency
        ) : MarketEvent(Symbol, Timestamp, Event);
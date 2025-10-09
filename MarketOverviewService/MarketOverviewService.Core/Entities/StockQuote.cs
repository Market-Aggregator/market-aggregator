using MarketOverviewService.Core.Entities.Enums;

namespace MarketOverviewService.Core.Entities;

public class StockQuote
{
    public long Id {get;set;}
    // public long StockQuoteId {get;set;}
    public required string Symbol {get;set;}
    public DateTimeOffset Timestamp {get;set;}
    public required string AskExchangeCode {get;set;}
    public decimal AskPrice {get;set;}
    public long AskSize {get;set;}
    public required string BidExchangeCode {get;set;}
    public decimal BidPrice {get;set;}
    public long BidSize {get;set;}
    public Currency Currency {get;set;}
};
namespace MarketAggregator.Core.Entities;

public class StockTrade
{
    public int StockTradeId {get;set;}
    public required string Symbol {get;set;}
    public required string Exchange {get;set;}
    public decimal Price {get;set;}
    public int Size {get;set;}
    public DateTimeOffset Timestamp {get;set;}
};
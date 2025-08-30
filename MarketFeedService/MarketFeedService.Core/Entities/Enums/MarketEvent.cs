namespace MarketFeedService.Core.Entities.Enums;

public enum MarketEvents
{
    Trade,
    Quote,
    Bar
}

[Flags]
public enum MarketFeeds
{
    None    = 0,
    Trades  = 1 << 0,
    Quotes  = 1 << 1,
    Bars    = 1 << 2,
}
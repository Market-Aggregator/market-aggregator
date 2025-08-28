namespace MarketFeedService.Core.Entities.Enums;

public enum MarketEvents {
    Trade,
    Quote,
    Bar
}

[Flags]
public enum MarketFeeds {
    None = 0,
    Trades = 1,
    Quotes = 2,
    Bars = 3
}
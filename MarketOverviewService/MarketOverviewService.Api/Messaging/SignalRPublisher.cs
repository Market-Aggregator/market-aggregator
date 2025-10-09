using MarketOverviewService.Api.Hubs;
using MarketOverviewService.Core.Entities;
using MarketOverviewService.Core.Entities.Enums;
using MarketOverviewService.Core.Interfaces;

using Microsoft.AspNetCore.SignalR;

namespace MarketOverviewService.Api.Messaging;

public class SignalRPublisher : IPublisher
{
    private readonly IHubContext<MarketHub> _hubContext;
    private readonly ILogger<SignalRPublisher> _logger;

    public SignalRPublisher(IHubContext<MarketHub> hubContext, ILogger<SignalRPublisher> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task BroadcastTradeAsync(StockTradeMessage trade)
    {
        var group = $"{MarketEvents.Trade}.{trade.ExchangeCode}.{trade.Symbol}";
        await _hubContext.Clients.Group(group).SendAsync("ReceiveTradeUpdate", trade);
        _logger.LogInformation("SignalRPublisher broadcasted message to group: {Group}", group);
    }

    public async Task BroadcastQuoteAsync(StockQuoteMessage quote) {
        var group = $"{MarketEvents.Quote}.{quote.Symbol}";
        await _hubContext.Clients.Group(group).SendAsync("ReceiveQuoteUpdate", quote);
        _logger.LogInformation("SignalRPublisher broadcasted message to group: {Group}", group);
    }
}
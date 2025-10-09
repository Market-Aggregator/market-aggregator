using MarketOverviewService.Core.Entities.Enums;

using Microsoft.AspNetCore.SignalR;

namespace MarketOverviewService.Api.Hubs;

public sealed class MarketHub : Hub
{
    private readonly ILogger<MarketHub> _logger;
    public MarketHub(ILogger<MarketHub> logger)
    {
        _logger = logger;
    }

    public async Task SubscribeToExchangeSymbol(string exchange, string symbol)
    {
        var group = $"{exchange}.{symbol}";
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        _logger.LogInformation("{ConnectionId} connected to group {Group}", Context.ConnectionId, group);
    }

    public async Task SubscribeToQuoteSymbol(string symbol) {
        var group = $"{MarketEvents.Quote}.{symbol}";
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        _logger.LogInformation("{ConnectionId} connected to group {Group}", Context.ConnectionId, group);
    }

    public async Task UnsubscribeFromExchangeSymbol(string exchange, string symbol)
    {
        var group = $"{exchange}.{symbol}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }

    public async Task UnsubscribeFromQuoteSymbol(string symbol) {
        var group = $"{MarketEvents.Quote}.{symbol}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
    }
}
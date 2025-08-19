using System.Net.WebSockets;
using System.Runtime.CompilerServices;

using MarketFeedService.Core.Entities.DataEntities;
using MarketFeedService.Core.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketFeedService.Infrastructure.Messaging.Adapters;

public class Finnhub : IMarketDataFeedAdapter
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Alpaca> _logger;

    public Finnhub(IConfiguration configuration, ILogger<Alpaca> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IAsyncEnumerable<StockTradeMessage> StreamAsync(IEnumerable<string> symbols, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
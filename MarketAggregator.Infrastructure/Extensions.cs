using Microsoft.Extensions.DependencyInjection;

using MarketAggregator.Core.Interfaces;
using MarketAggregator.Infrastructure.Repositories.Adapters;

namespace MarketAggregator.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddIngestorInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ILiveMarketDataClient, AlpacaWebSocketClient>();

        return services;
    }
}
using Microsoft.Extensions.DependencyInjection;

using MarketAggregator.Core.Interfaces;
using MarketAggregator.Infrastructure.Repositories.Adapters;
using MarketAggregator.Infrastructure.Repositories.Producers;

namespace MarketAggregator.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddIngestorInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IStockTradeProducer, KafkaStockTradeProducer>();
        services.AddSingleton<ILiveMarketDataClient, AlpacaWebSocketClient>();

        return services;
    }
}
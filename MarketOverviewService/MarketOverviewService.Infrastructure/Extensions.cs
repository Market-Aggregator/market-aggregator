using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Configuration;
using MarketOverviewService.Infrastructure.Messaging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOverviewService.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton<ILiveMarketDataConsumer, KafkaStockTradeConsumer>();

        return services;
    }
}
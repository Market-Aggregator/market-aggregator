using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Configuration;
using MarketOverviewService.Infrastructure.Messaging;
using MarketOverviewService.Infrastructure.Persistence.Data;
using MarketOverviewService.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketOverviewService.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<AppDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString("PostgresConnection"
                ))
        );
        services.AddScoped<IStockTradeRepository, EfStockTradeRepository>();

        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton<IMarketDataConsumer, KafkaStockTradeConsumer>();

        return services;
    }
}
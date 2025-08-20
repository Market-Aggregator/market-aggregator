using Confluent.Kafka;

using MarketFeedService.Core.Interfaces;
using MarketFeedService.Infrastructure.Configuration;
using MarketFeedService.Infrastructure.Messaging.Adapters;
using MarketFeedService.Infrastructure.Messaging.Producers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MarketFeedService.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));

        services.AddSingleton(sp =>
                {
                    var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

                    var config = new ProducerConfig
                    {
                        BootstrapServers = kafkaSettings.BootstrapServers,
                        Acks = Acks.All,
                        EnableIdempotence = true,
                        MessageSendMaxRetries = 3,
                        RetryBackoffMs = 100
                    };

                    return new ProducerBuilder<string, string>(config).Build();
                });
        services.AddSingleton<IStockTradeProducer, KafkaStockTradeProducer>();
        services.AddKeyedSingleton<IMarketDataFeedAdapter, Alpaca>("alpaca");
        services.AddKeyedSingleton<IMarketDataFeedAdapter, AlpacaTest>("alpacatest");
        services.AddKeyedSingleton<IMarketDataFeedAdapter, YahooFinance>("yahoofinance");

        return services;
    }
}
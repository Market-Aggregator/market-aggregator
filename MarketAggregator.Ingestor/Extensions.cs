using Confluent.Kafka;

using MarketAggregator.Ingestor.Configurations;

using Microsoft.Extensions.Options;

namespace MarketAggregator.Ingestor;

public static class IngestorExtensions
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
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

                    return new ProducerBuilder<Null, string>(config).Build();
                });

        return services;
    }
}
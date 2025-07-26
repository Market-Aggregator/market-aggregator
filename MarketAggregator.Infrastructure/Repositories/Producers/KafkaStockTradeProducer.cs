
using Confluent.Kafka;

using MarketAggregator.Core.Interfaces;

namespace MarketAggregator.Infrastructure.Repositories.Producers;

public class KafkaStockTradeProducer : IStockTradeProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaStockTradeProducer(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public Task ProduceAsync(string topic, string key, string payload, CancellationToken ct)
    {
        return _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = payload }, ct);
    }
}
using Confluent.Kafka;

using MarketFeedService.Core.Interfaces;

namespace MarketFeedService.Infrastructure.Messaging.Producers;

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

    public void Flush(CancellationToken ct)
    {
        _producer.Flush(ct);
    }
}
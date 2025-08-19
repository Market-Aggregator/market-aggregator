using System.Runtime.CompilerServices;
using System.Text.Json;

using Confluent.Kafka;

using MarketOverviewService.Core.Entities;
using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketOverviewService.Infrastructure.Messaging;

public class KafkaStockTradeConsumer : IMarketDataConsumer
{
    private readonly ILogger<KafkaStockTradeConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IConsumer<string, string> _consumer;

    public KafkaStockTradeConsumer(ILogger<KafkaStockTradeConsumer> logger, IOptions<KafkaSettings> kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async IAsyncEnumerable<StockTradeMessage> ConsumeAsync(string exchange, [EnumeratorCancellation] CancellationToken ct)
    {
        _consumer.Subscribe(exchange);
        _logger.LogInformation("Kafka Consumer started and subscribed to topic: {Topic}", exchange);

        while (!ct.IsCancellationRequested)
        {
            var result = _consumer.Consume(ct);
            if (result is null || string.IsNullOrWhiteSpace(result.Message?.Value)) continue;

            var stockTrade = JsonSerializer.Deserialize<StockTradeMessage>(result.Message.Value);

            if (stockTrade is not null)
            {
                yield return stockTrade;
            }

            await Task.Yield();
        }
    }
}
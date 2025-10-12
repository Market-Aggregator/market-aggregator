using System.Runtime.CompilerServices;
using System.Text.Json;

using Confluent.Kafka;

using MarketOverviewService.Core.Entities;
using MarketOverviewService.Core.Entities.Enums;
using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarketOverviewService.Infrastructure.Messaging;

// TODO: make this into a generic class to support different asset classes
// i.e. stocks, ETFs, commodities, crypto etc.
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

        _consumer = new ConsumerBuilder<string, string>(config).SetPartitionsAssignedHandler((c, partitions) =>
            {
                _logger.LogInformation("✅ Partitions assigned: {Partitions}", 
                    string.Join(", ", partitions.Select(p => $"{p.Topic}[{p.Partition.Value}]")));
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                _logger.LogInformation("⚠️ Partitions revoked: {Partitions}", 
                    string.Join(", ", partitions.Select(p => $"{p.Topic}[{p.Partition.Value}]")));
            }).Build();
    }

    public async IAsyncEnumerable<MarketEvent> ConsumeAsync(IEnumerable<string> topics, [EnumeratorCancellation] CancellationToken ct)
    {
        _consumer.Subscribe(topics);
        _logger.LogInformation("🟢 Subscribed to topics: {Topics}", string.Join(", ", topics));
        _logger.LogInformation("🧩 Consumer GroupId: {GroupId}", _kafkaSettings.GroupId);

        while (!ct.IsCancellationRequested)
        {
            var message = _consumer.Consume(ct);
            if (message is null || string.IsNullOrWhiteSpace(message.Message?.Value)) continue;
            
            _logger.LogInformation("📩 Message received on topic '{Topic}' partition {Partition} @ offset {Offset}",
                message.Topic, message.Partition.Value, message.Offset.Value);

            var messageJson = JObject.Parse(message.Message.Value);
            if (!Enum.TryParse<MarketEvents>((string)messageJson["Event"]!, ignoreCase: true, out var eventType))
            {
                _logger.LogWarning("⚠️ Unknown event type in message: {Message}", message.Message.Value);
                continue;
            }
            
            _logger.LogInformation("Event type parsed: {EventType}", eventType);

            MarketEvent? marketEvent = null;
            if (eventType == MarketEvents.Trade)
            {
                marketEvent = JsonConvert.DeserializeObject<StockTradeMessage>(message.Message.Value);
            }
            else if (eventType == MarketEvents.Quote)
            {
                marketEvent = JsonConvert.DeserializeObject<StockQuoteMessage>(message.Message.Value);
            }

            if (marketEvent is not null) yield return marketEvent;
        }
    }
}
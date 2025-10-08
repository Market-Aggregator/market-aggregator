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

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    public async IAsyncEnumerable<MarketEvent> ConsumeAsync(string exchange, [EnumeratorCancellation] CancellationToken ct)
    {
        _consumer.Subscribe(exchange);
        _logger.LogInformation("Kafka Consumer started and subscribed to topic: {Topic}", exchange);

        while (!ct.IsCancellationRequested)
        {
            var message = _consumer.Consume(ct);
            if (message is null || string.IsNullOrWhiteSpace(message.Message?.Value)) continue;

            var messageJson = JObject.Parse(message.Message.Value);
            if (!Enum.TryParse<MarketEvents>((string)messageJson["Event"]!, out var eventType)) continue;

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
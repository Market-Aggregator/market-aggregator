
using System.Text.Json;

using Confluent.Kafka;

using MarketOverviewService.Core.Entities;
using MarketOverviewService.Core.Interfaces;
using MarketOverviewService.Infrastructure.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarketOverviewService.Infrastructure.Messaging;

public class KafkaStockTradeConsumer : ILiveMarketDataConsumer
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

    public async Task<StockTrade?> ConsumeAsync(CancellationToken ct)
    {

        await Task.Yield();

        // TODO: remove hardcoded topic
        string topic = "N.FAKEPACA";
        _consumer.Subscribe(topic);
        _logger.LogInformation("Kafka Consumer started and subscribed to topic: {Topic}", topic);

        var result = _consumer.Consume(ct);
        if (result is null || string.IsNullOrWhiteSpace(result.Message?.Value)) return null;

        var stockTrade = JsonSerializer.Deserialize<StockTrade>(result.Message.Value);

        if (stockTrade is null)
        {
            _logger.LogWarning("Receied null or malformed stock trade event");
            return null;
        }

        _logger.LogInformation("Stock Trade received: {StockTradeSymbol} {StockTradeId} at {StockTradeTimestamp}", stockTrade.Symbol, stockTrade.StockTradeId, stockTrade.Timestamp);
        return stockTrade;
    }
}
using System.Text.Json.Serialization;

namespace MarketFeedService.Core.Entities.ApiEntities;

public sealed class FinnhubTrade {
    [JsonPropertyName("data")]
    public IEnumerable<FinnhubTradeData?> Data {get;set;}

    [JsonPropertyName("type")]
    public required string Type {get;set;}
}

public sealed class FinnhubTradeData {
    [JsonPropertyName("s")]
    public required string Symbol {get;set;}
    
    [JsonPropertyName("p")]
    public decimal Price {get;set;}

    [JsonPropertyName("t")]
    public long EpochTimestamp {get;set;}

    [JsonPropertyName("v")]
    public decimal Volume {get;set;}

    [JsonPropertyName("c")]
    public IEnumerable<string>? Conditions {get;set;}

    [JsonIgnore]
    public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeMilliseconds(EpochTimestamp);
}
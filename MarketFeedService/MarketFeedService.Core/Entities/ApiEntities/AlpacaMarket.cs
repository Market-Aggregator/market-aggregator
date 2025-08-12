using System.Text.Json.Serialization;

namespace MarketFeedService.Core.Entities.ApiEntities;

public sealed class TradeResponse {
    [JsonPropertyName("T")] 
    public required string Type {get;set;}
    [JsonPropertyName("S")] 
    public required string Symbol {get;set;}
    [JsonPropertyName("i")] 
    public int TradeId {get;set;}
    [JsonPropertyName("x")] 
    public required string ExchangeCode {get;set;}
    [JsonPropertyName("p")] 
    public decimal Price {get;set;}
    [JsonPropertyName("s")]
    public int Size {get;set;}
    [JsonPropertyName("c")] 
    public required IEnumerable<string> Conditions {get;set;}
    [JsonPropertyName("t")] 
    public DateTimeOffset Timestamp {get;set;}
    [JsonPropertyName("z")] 
    public required string Tape {get;set;}
}
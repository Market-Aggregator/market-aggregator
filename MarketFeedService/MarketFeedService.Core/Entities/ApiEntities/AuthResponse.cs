using System.Text.Json.Serialization;

namespace MarketFeedService.Core.Entities.ApiEntities;

public class AuthResponse
{
    [JsonPropertyName("T")]
    public required string Type { get; set; }
    [JsonPropertyName("msg")]
    public required string Msg { get; set; }
}
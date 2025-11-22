using System.Text.Json.Serialization;

namespace ElasticSearchDotNet.Api.Models;

public class Neighbor
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("town_code")]
    public string TownCode { get; set; } = string.Empty; // District code
}


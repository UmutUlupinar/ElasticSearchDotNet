using System.Text.Json.Serialization;

namespace ElasticSearchDotNet.Api.Models;

public class City
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}


using System.Text.Json.Serialization;

namespace ElasticSearchDotNet.Api.Models;

public class District
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("city_code")]
    public string CityCode { get; set; } = string.Empty;
}


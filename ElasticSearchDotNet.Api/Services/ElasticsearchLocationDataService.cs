using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ElasticSearchDotNet.Api.Models;

namespace ElasticSearchDotNet.Api.Services;

public class ElasticsearchLocationDataService : ILocationDataService
{
    private readonly ElasticsearchClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ElasticsearchLocationDataService> _logger;
    private readonly string _indexPrefix;

    private const string CitiesIndex = "cities";
    private const string DistrictsIndex = "districts";
    private const string NeighborsIndex = "neighbors";

    public ElasticsearchLocationDataService(
        ElasticsearchClient client,
        IConfiguration configuration,
        ILogger<ElasticsearchLocationDataService> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
        _indexPrefix = _configuration["Elasticsearch:IndexPrefix"] ?? "location";
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        try
        {
            var indexName = $"{_indexPrefix}-{CitiesIndex}";
            var response = await _client.SearchAsync<City>(s => s
                .Indices(indexName)
                .Size(10000)
            );

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            _logger.LogWarning("Failed to get cities from Elasticsearch: {Error}", response.DebugInformation);
            return Enumerable.Empty<City>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cities from Elasticsearch");
            return Enumerable.Empty<City>();
        }
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync()
    {
        try
        {
            var indexName = $"{_indexPrefix}-{DistrictsIndex}";
            var response = await _client.SearchAsync<District>(s => s
                .Indices(indexName)
                .Size(10000)
            );

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            _logger.LogWarning("Failed to get districts from Elasticsearch: {Error}", response.DebugInformation);
            return Enumerable.Empty<District>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting districts from Elasticsearch");
            return Enumerable.Empty<District>();
        }
    }

    public async Task<IEnumerable<Neighbor>> GetNeighborsAsync()
    {
        try
        {
            var indexName = $"{_indexPrefix}-{NeighborsIndex}";
            var response = await _client.SearchAsync<Neighbor>(s => s
                .Indices(indexName)
                .Size(100000)
            );

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            _logger.LogWarning("Failed to get neighbors from Elasticsearch: {Error}", response.DebugInformation);
            return Enumerable.Empty<Neighbor>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting neighbors from Elasticsearch");
            return Enumerable.Empty<Neighbor>();
        }
    }

    public async Task<IEnumerable<City>> SearchCitiesAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetCitiesAsync();

            var indexName = $"{_indexPrefix}-{CitiesIndex}";
            var response = await _client.SearchAsync<City>(s => s
                .Indices(indexName)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Description)
                        .Query(searchTerm)
                        .Fuzziness(new Fuzziness("AUTO"))
                    )
                )
                .Size(100)
            );

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            _logger.LogWarning("Failed to search cities in Elasticsearch: {Error}", response.DebugInformation);
            return Enumerable.Empty<City>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching cities in Elasticsearch");
            return Enumerable.Empty<City>();
        }
    }

    public async Task<IEnumerable<District>> SearchDistrictsAsync(string searchTerm, string? cityCode = null)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{DistrictsIndex}";
            
            var searchDescriptor = new SearchRequestDescriptor<District>()
                .Indices(indexName)
                .Size(1000);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchDescriptor = searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(ma => ma
                                .Field(f => f.Description)
                                .Query(searchTerm)
                                .Fuzziness(new Fuzziness("AUTO"))
                            )
                        )
                        .Filter(!string.IsNullOrWhiteSpace(cityCode) ? new[] { (Action<QueryDescriptor<District>>)(f => f
                            .Term(t => t
                                .Field(f => f.CityCode)
                                .Value(cityCode!)
                            )) } : Array.Empty<Action<QueryDescriptor<District>>>()
                        )
                    )
                );
            }
            else if (!string.IsNullOrWhiteSpace(cityCode))
            {
                searchDescriptor = searchDescriptor.Query(q => q
                    .Term(t => t
                        .Field(f => f.CityCode)
                        .Value(cityCode)
                    )
                );
            }
            else
            {
                return await GetDistrictsAsync();
            }

            var response = await _client.SearchAsync<District>(searchDescriptor);

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            _logger.LogWarning("Failed to search districts in Elasticsearch: {Error}", response.DebugInformation);
            return Enumerable.Empty<District>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching districts in Elasticsearch");
            return Enumerable.Empty<District>();
        }
    }

    public async Task<IEnumerable<Neighbor>> SearchNeighborsAsync(string searchTerm, string? districtCode = null)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{NeighborsIndex}";
            
            var searchDescriptor = new SearchRequestDescriptor<Neighbor>()
                .Indices(indexName)
                .Size(5000);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchDescriptor = searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(ma => ma
                                .Field(f => f.Description)
                                .Query(searchTerm)
                                .Fuzziness(new Fuzziness("AUTO"))
                            )
                        )
                        .Filter(!string.IsNullOrWhiteSpace(districtCode) ? new[] { (Action<QueryDescriptor<Neighbor>>)(f => f
                            .Term(t => t
                                .Field(f => f.TownCode)
                                .Value(districtCode!)
                            )) } : Array.Empty<Action<QueryDescriptor<Neighbor>>>()
                        )
                    )
                );
            }
            else if (!string.IsNullOrWhiteSpace(districtCode))
            {
                searchDescriptor = searchDescriptor.Query(q => q
                    .Term(t => t
                        .Field(f => f.TownCode)
                        .Value(districtCode)
                    )
                );
            }
            else
            {
                return await GetNeighborsAsync();
            }

            var response = await _client.SearchAsync<Neighbor>(searchDescriptor);

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            _logger.LogWarning("Failed to search neighbors in Elasticsearch: {Error}", response.DebugInformation);
            return Enumerable.Empty<Neighbor>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching neighbors in Elasticsearch");
            return Enumerable.Empty<Neighbor>();
        }
    }

    public async Task<City?> GetCityByCodeAsync(string code)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{CitiesIndex}";
            var response = await _client.SearchAsync<City>(s => s
                .Indices(indexName)
                .Query(q => q.Term(t => t.Field(f => f.Code).Value(code)))
                .Size(1)
            );

            if (response.IsValidResponse && response.Documents.Any())
            {
                return response.Documents.First();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting city by code from Elasticsearch: {Code}", code);
            return null;
        }
    }

    public async Task<District?> GetDistrictByCodeAsync(string code)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{DistrictsIndex}";
            var response = await _client.SearchAsync<District>(s => s
                .Indices(indexName)
                .Query(q => q.Term(t => t.Field(f => f.Code).Value(code)))
                .Size(1)
            );

            if (response.IsValidResponse && response.Documents.Any())
            {
                return response.Documents.First();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting district by code from Elasticsearch: {Code}", code);
            return null;
        }
    }

    public async Task<IEnumerable<District>> GetDistrictsByCityCodeAsync(string cityCode)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{DistrictsIndex}";
            var response = await _client.SearchAsync<District>(s => s
                .Indices(indexName)
                .Query(q => q.Term(t => t.Field(f => f.CityCode).Value(cityCode)))
                .Size(1000)
            );

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            return Enumerable.Empty<District>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting districts by city code from Elasticsearch: {CityCode}", cityCode);
            return Enumerable.Empty<District>();
        }
    }

    public async Task<IEnumerable<Neighbor>> GetNeighborsByDistrictCodeAsync(string districtCode)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{NeighborsIndex}";
            var response = await _client.SearchAsync<Neighbor>(s => s
                .Indices(indexName)
                .Query(q => q.Term(t => t.Field(f => f.TownCode).Value(districtCode)))
                .Size(10000)
            );

            if (response.IsValidResponse)
            {
                return response.Documents;
            }

            return Enumerable.Empty<Neighbor>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting neighbors by district code from Elasticsearch: {DistrictCode}", districtCode);
            return Enumerable.Empty<Neighbor>();
        }
    }
}

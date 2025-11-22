using Elastic.Clients.Elasticsearch;
using ElasticSearchDotNet.Api.Models;

namespace ElasticSearchDotNet.Api.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly string _indexPrefix;

    private const string CitiesIndex = "cities";
    private const string DistrictsIndex = "districts";
    private const string NeighborsIndex = "neighbors";

    public ElasticsearchService(
        ElasticsearchClient client,
        IConfiguration configuration,
        ILogger<ElasticsearchService> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
        _indexPrefix = _configuration["Elasticsearch:IndexPrefix"] ?? "location";
    }

    public async Task<bool> CreateIndicesAsync()
    {
        try
        {
            // Önce bağlantıyı test et
            var pingResponse = await _client.PingAsync();
            if (!pingResponse.IsValidResponse)
            {
                _logger.LogError("Elasticsearch bağlantısı yok: {Error}", pingResponse.DebugInformation);
                return false;
            }

            // Cities Index - Basit mapping ile
            var citiesIndexName = $"{_indexPrefix}-{CitiesIndex}";
            var citiesExists = await _client.Indices.ExistsAsync(citiesIndexName);
            
            if (!citiesExists.Exists)
            {
                var citiesResponse = await _client.Indices.CreateAsync(citiesIndexName);
                if (citiesResponse.IsValidResponse)
                {
                    _logger.LogInformation("Cities index created: {IndexName}", citiesIndexName);
                }
                else
                {
                    _logger.LogError("Failed to create cities index: {Error}. Debug: {DebugInfo}", 
                        citiesResponse.ElasticsearchServerError?.Error?.Reason, 
                        citiesResponse.DebugInformation);
                    return false;
                }
            }
            else
            {
                _logger.LogInformation("Cities index already exists: {IndexName}", citiesIndexName);
            }

            // Districts Index
            var districtsIndexName = $"{_indexPrefix}-{DistrictsIndex}";
            var districtsExists = await _client.Indices.ExistsAsync(districtsIndexName);
            
            if (!districtsExists.Exists)
            {
                var districtsResponse = await _client.Indices.CreateAsync(districtsIndexName);
                if (districtsResponse.IsValidResponse)
                {
                    _logger.LogInformation("Districts index created: {IndexName}", districtsIndexName);
                }
                else
                {
                    _logger.LogError("Failed to create districts index: {Error}. Debug: {DebugInfo}", 
                        districtsResponse.ElasticsearchServerError?.Error?.Reason, 
                        districtsResponse.DebugInformation);
                    return false;
                }
            }
            else
            {
                _logger.LogInformation("Districts index already exists: {IndexName}", districtsIndexName);
            }

            // Neighbors Index
            var neighborsIndexName = $"{_indexPrefix}-{NeighborsIndex}";
            var neighborsExists = await _client.Indices.ExistsAsync(neighborsIndexName);
            
            if (!neighborsExists.Exists)
            {
                var neighborsResponse = await _client.Indices.CreateAsync(neighborsIndexName);
                if (neighborsResponse.IsValidResponse)
                {
                    _logger.LogInformation("Neighbors index created: {IndexName}", neighborsIndexName);
                }
                else
                {
                    _logger.LogError("Failed to create neighbors index: {Error}. Debug: {DebugInfo}", 
                        neighborsResponse.ElasticsearchServerError?.Error?.Reason, 
                        neighborsResponse.DebugInformation);
                    return false;
                }
            }
            else
            {
                _logger.LogInformation("Neighbors index already exists: {IndexName}", neighborsIndexName);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating indices");
            return false;
        }
    }

    public async Task<bool> IndexCitiesAsync(IEnumerable<City> cities)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{CitiesIndex}";
            var bulkResponse = await _client.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(cities)
            );

            if (bulkResponse.IsValidResponse)
            {
                _logger.LogInformation("Indexed {Count} cities", cities.Count());
                return true;
            }
            else
            {
                _logger.LogError("Failed to index cities: {Error}", bulkResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing cities");
            return false;
        }
    }

    public async Task<bool> IndexDistrictsAsync(IEnumerable<District> districts)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{DistrictsIndex}";
            var bulkResponse = await _client.BulkAsync(b => b
                .Index(indexName)
                .IndexMany(districts)
            );

            if (bulkResponse.IsValidResponse)
            {
                _logger.LogInformation("Indexed {Count} districts", districts.Count());
                return true;
            }
            else
            {
                _logger.LogError("Failed to index districts: {Error}", bulkResponse.DebugInformation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing districts");
            return false;
        }
    }

    public async Task<bool> IndexNeighborsAsync(IEnumerable<Neighbor> neighbors)
    {
        try
        {
            var indexName = $"{_indexPrefix}-{NeighborsIndex}";
            
            // Büyük veri setleri için batch'ler halinde indexleme
            const int batchSize = 1000;
            var neighborList = neighbors.ToList();
            var totalBatches = (int)Math.Ceiling(neighborList.Count / (double)batchSize);
            var successCount = 0;

            for (int i = 0; i < neighborList.Count; i += batchSize)
            {
                var batch = neighborList.Skip(i).Take(batchSize);
                var bulkResponse = await _client.BulkAsync(b => b
                    .Index(indexName)
                    .IndexMany(batch)
                );

                if (bulkResponse.IsValidResponse)
                {
                    successCount += batch.Count();
                    var currentBatch = (i / batchSize) + 1;
                    _logger.LogInformation("Indexed batch {CurrentBatch}/{TotalBatches} ({Count} neighbors)", 
                        currentBatch, totalBatches, batch.Count());
                }
                else
                {
                    _logger.LogError("Failed to index neighbors batch: {Error}", bulkResponse.DebugInformation);
                }
            }

            _logger.LogInformation("Indexed {SuccessCount}/{TotalCount} neighbors", successCount, neighborList.Count);
            return successCount == neighborList.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing neighbors");
            return false;
        }
    }

    public async Task<bool> ReindexAllAsync()
    {
        // Bu metod dışarıdan veri alacak şekilde tasarlandı
        // Veriler ILocationDataService üzerinden gelecek
        throw new NotImplementedException("Use IndexCitiesAsync, IndexDistrictsAsync, IndexNeighborsAsync methods with data from ILocationDataService");
    }
}

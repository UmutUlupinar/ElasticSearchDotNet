using ElasticSearchDotNet.Api.Models;

namespace ElasticSearchDotNet.Api.Services;

public interface IElasticsearchService
{
    Task<bool> CreateIndicesAsync();
    Task<bool> IndexCitiesAsync(IEnumerable<City> cities);
    Task<bool> IndexDistrictsAsync(IEnumerable<District> districts);
    Task<bool> IndexNeighborsAsync(IEnumerable<Neighbor> neighbors);
    Task<bool> ReindexAllAsync();
}


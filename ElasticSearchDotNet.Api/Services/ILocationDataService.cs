using ElasticSearchDotNet.Api.Models;

namespace ElasticSearchDotNet.Api.Services;

public interface ILocationDataService
{
    Task<IEnumerable<City>> GetCitiesAsync();
    Task<IEnumerable<District>> GetDistrictsAsync();
    Task<IEnumerable<Neighbor>> GetNeighborsAsync();
    
    Task<IEnumerable<City>> SearchCitiesAsync(string searchTerm);
    Task<IEnumerable<District>> SearchDistrictsAsync(string searchTerm, string? cityCode = null);
    Task<IEnumerable<Neighbor>> SearchNeighborsAsync(string searchTerm, string? districtCode = null);
    
    Task<City?> GetCityByCodeAsync(string code);
    Task<District?> GetDistrictByCodeAsync(string code);
    Task<IEnumerable<District>> GetDistrictsByCityCodeAsync(string cityCode);
    Task<IEnumerable<Neighbor>> GetNeighborsByDistrictCodeAsync(string districtCode);
}


using System.Text.Json;
using ElasticSearchDotNet.Api.Models;

namespace ElasticSearchDotNet.Api.Services;

public class LocationDataService : ILocationDataService
{
    private readonly ILogger<LocationDataService> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly Lazy<Task<List<City>>> _cities;
    private readonly Lazy<Task<List<District>>> _districts;
    private readonly Lazy<Task<List<Neighbor>>> _neighbors;

    public LocationDataService(ILogger<LocationDataService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
        
        // Lazy loading - veriler ilk kullanımda yüklenecek
        _cities = new Lazy<Task<List<City>>>(() => LoadCitiesAsync());
        _districts = new Lazy<Task<List<District>>>(() => LoadDistrictsAsync());
        _neighbors = new Lazy<Task<List<Neighbor>>>(() => LoadNeighborsAsync());
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _cities.Value;
    }

    public async Task<IEnumerable<District>> GetDistrictsAsync()
    {
        return await _districts.Value;
    }

    public async Task<IEnumerable<Neighbor>> GetNeighborsAsync()
    {
        return await _neighbors.Value;
    }

    public async Task<IEnumerable<City>> SearchCitiesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetCitiesAsync();

        var cities = await _cities.Value;
        var term = searchTerm.Trim().ToUpperInvariant();
        
        return cities.Where(c => 
            c.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            c.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
        );
    }

    public async Task<IEnumerable<District>> SearchDistrictsAsync(string searchTerm, string? cityCode = null)
    {
        var districts = await _districts.Value;
        
        if (!string.IsNullOrWhiteSpace(cityCode))
        {
            districts = districts.Where(d => d.CityCode == cityCode).ToList();
        }

        if (string.IsNullOrWhiteSpace(searchTerm))
            return districts;

        var term = searchTerm.Trim().ToUpperInvariant();
        
        return districts.Where(d => 
            d.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            d.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
        );
    }

    public async Task<IEnumerable<Neighbor>> SearchNeighborsAsync(string searchTerm, string? districtCode = null)
    {
        var neighbors = await _neighbors.Value;
        
        if (!string.IsNullOrWhiteSpace(districtCode))
        {
            neighbors = neighbors.Where(n => n.TownCode == districtCode).ToList();
        }

        if (string.IsNullOrWhiteSpace(searchTerm))
            return neighbors;

        var term = searchTerm.Trim().ToUpperInvariant();
        
        return neighbors.Where(n => 
            n.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            n.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
        );
    }

    public async Task<City?> GetCityByCodeAsync(string code)
    {
        var cities = await _cities.Value;
        return cities.FirstOrDefault(c => c.Code == code);
    }

    public async Task<District?> GetDistrictByCodeAsync(string code)
    {
        var districts = await _districts.Value;
        return districts.FirstOrDefault(d => d.Code == code);
    }

    public async Task<IEnumerable<District>> GetDistrictsByCityCodeAsync(string cityCode)
    {
        var districts = await _districts.Value;
        return districts.Where(d => d.CityCode == cityCode);
    }

    public async Task<IEnumerable<Neighbor>> GetNeighborsByDistrictCodeAsync(string districtCode)
    {
        var neighbors = await _neighbors.Value;
        return neighbors.Where(n => n.TownCode == districtCode);
    }

    private async Task<List<City>> LoadCitiesAsync()
    {
        try
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "data-city.json");
            
            if (!File.Exists(filePath))
            {
                _logger.LogError("City data file not found: {FilePath}", filePath);
                return new List<City>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            var cities = JsonSerializer.Deserialize<List<City>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Loaded {Count} cities from data file", cities?.Count ?? 0);
            return cities ?? new List<City>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cities data");
            return new List<City>();
        }
    }

    private async Task<List<District>> LoadDistrictsAsync()
    {
        try
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "data-district.json");
            
            if (!File.Exists(filePath))
            {
                _logger.LogError("District data file not found: {FilePath}", filePath);
                return new List<District>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            var districts = JsonSerializer.Deserialize<List<District>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Loaded {Count} districts from data file", districts?.Count ?? 0);
            return districts ?? new List<District>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading districts data");
            return new List<District>();
        }
    }

    private async Task<List<Neighbor>> LoadNeighborsAsync()
    {
        try
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "data-neighbors.json");
            
            if (!File.Exists(filePath))
            {
                _logger.LogError("Neighbor data file not found: {FilePath}", filePath);
                return new List<Neighbor>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            var neighbors = JsonSerializer.Deserialize<List<Neighbor>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Loaded {Count} neighbors from data file", neighbors?.Count ?? 0);
            return neighbors ?? new List<Neighbor>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading neighbors data");
            return new List<Neighbor>();
        }
    }
}


using System.Net.Http.Json;

namespace ElasticSearchDotNet.Web.Services;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiService> _logger;

    public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            _logger.LogWarning("API call failed: {StatusCode} - {Endpoint}", response.StatusCode, endpoint);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API endpoint: {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            _logger.LogWarning("API POST call failed: {StatusCode} - {Endpoint}", response.StatusCode, endpoint);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API POST endpoint: {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PutAsJsonAsync(endpoint, data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            _logger.LogWarning("API PUT call failed: {StatusCode} - {Endpoint}", response.StatusCode, endpoint);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API PUT endpoint: {Endpoint}", endpoint);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            _logger.LogWarning("API DELETE call failed: {StatusCode} - {Endpoint}", response.StatusCode, endpoint);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API DELETE endpoint: {Endpoint}", endpoint);
            return false;
        }
    }
}


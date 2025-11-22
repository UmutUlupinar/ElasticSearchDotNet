using Elastic.Clients.Elasticsearch;
using ElasticSearchDotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchDotNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElasticsearchHealthController : ControllerBase
{
    private readonly ElasticsearchClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ElasticsearchHealthController> _logger;

    public ElasticsearchHealthController(
        ElasticsearchClient client,
        IConfiguration configuration,
        ILogger<ElasticsearchHealthController> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Elasticsearch bağlantısını test eder
    /// </summary>
    [HttpGet("test-connection")]
    public async Task<ActionResult<ApiResponse<object>>> TestConnection()
    {
         try
    {
        var elasticsearchUri = _configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";

        // Ping yerine Info kullanıyoruz
        var infoResponse = await _client.InfoAsync();

        if (infoResponse.IsValidResponse)
        {
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                connected = true,
                uri = elasticsearchUri,
                clusterName = infoResponse.ClusterName,
                version = infoResponse.Version?.Number
            }, "Elasticsearch bağlantısı başarılı"));
        }
        else
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                $"Elasticsearch bağlantısı başarısız: {infoResponse.DebugInformation}"
            ));
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error testing Elasticsearch connection");
        return StatusCode(500, ApiResponse<object>.ErrorResponse(
            $"Elasticsearch bağlantı hatası: {ex.Message}"
        ));
    }
    }

    /// <summary>
    /// Elasticsearch cluster durumunu kontrol eder
    /// </summary>
    [HttpGet("cluster-health")]
    public async Task<ActionResult<ApiResponse<object>>> GetClusterHealth()
    {
        try
        {
            var healthResponse = await _client.Cluster.HealthAsync();
            
            if (healthResponse.IsValidResponse)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    status = healthResponse.Status.ToString(),
                    clusterName = healthResponse.ClusterName,
                    numberOfNodes = healthResponse.NumberOfNodes,
                    numberOfDataNodes = healthResponse.NumberOfDataNodes,
                    activePrimaryShards = healthResponse.ActivePrimaryShards,
                    activeShards = healthResponse.ActiveShards,
                    relocatingShards = healthResponse.RelocatingShards,
                    initializingShards = healthResponse.InitializingShards,
                    unassignedShards = healthResponse.UnassignedShards
                }, "Cluster durumu alındı"));
            }
            else
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Cluster durumu alınamadı: {healthResponse.DebugInformation}"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cluster health");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"Cluster durumu alınırken hata: {ex.Message}"));
        }
    }
}


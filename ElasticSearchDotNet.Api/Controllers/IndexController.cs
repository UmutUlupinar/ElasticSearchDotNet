using ElasticSearchDotNet.Api.Models;
using ElasticSearchDotNet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchDotNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndexController : ControllerBase
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILocationDataService _locationDataService;
    private readonly ILogger<IndexController> _logger;

    public IndexController(
        IElasticsearchService elasticsearchService,
        ILocationDataService locationDataService,
        ILogger<IndexController> logger)
    {
        _elasticsearchService = elasticsearchService;
        _locationDataService = locationDataService;
        _logger = logger;
    }

    /// <summary>
    /// Elasticsearch index'lerini oluşturur
    /// </summary>
    [HttpPost("create-indices")]
    public async Task<ActionResult<ApiResponse<object>>> CreateIndices()
    {
        try
        {
            var result = await _elasticsearchService.CreateIndicesAsync();
            
            if (result)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Index'ler başarıyla oluşturuldu" }, "Index'ler başarıyla oluşturuldu"));
            }
            else
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Index'ler oluşturulurken bir hata oluştu"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating indices");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Index'ler oluşturulurken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Tüm verileri Elasticsearch'e indexler (JSON dosyalarından)
    /// </summary>
    [HttpPost("index-all")]
    public async Task<ActionResult<ApiResponse<object>>> IndexAll()
    {
        try
        {
            // Önce index'leri oluştur
            var indicesCreated = await _elasticsearchService.CreateIndicesAsync();
            if (!indicesCreated)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Index'ler oluşturulamadı"));
            }

            // JSON dosyalarından verileri yükle (LocationDataService kullan)
            // Not: Bu işlem için geçici olarak JSON tabanlı servisi kullanıyoruz
            var jsonDataService = new LocationDataService(
                HttpContext.RequestServices.GetRequiredService<ILogger<LocationDataService>>(),
                HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>()
            );

            var cities = await jsonDataService.GetCitiesAsync();
            var districts = await jsonDataService.GetDistrictsAsync();
            var neighbors = await jsonDataService.GetNeighborsAsync();

            // Verileri indexle
            var citiesIndexed = await _elasticsearchService.IndexCitiesAsync(cities);
            var districtsIndexed = await _elasticsearchService.IndexDistrictsAsync(districts);
            var neighborsIndexed = await _elasticsearchService.IndexNeighborsAsync(neighbors);

            if (citiesIndexed && districtsIndexed && neighborsIndexed)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    citiesCount = cities.Count(),
                    districtsCount = districts.Count(),
                    neighborsCount = neighbors.Count()
                }, "Tüm veriler başarıyla indexlendi"));
            }
            else
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Veriler indexlenirken bir hata oluştu"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing all data");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Veriler indexlenirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Şehirleri indexler
    /// </summary>
    [HttpPost("index-cities")]
    public async Task<ActionResult<ApiResponse<object>>> IndexCities()
    {
        try
        {
            var jsonDataService = new LocationDataService(
                HttpContext.RequestServices.GetRequiredService<ILogger<LocationDataService>>(),
                HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>()
            );

            var cities = await jsonDataService.GetCitiesAsync();
            var result = await _elasticsearchService.IndexCitiesAsync(cities);

            if (result)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { count = cities.Count() }, "Şehirler başarıyla indexlendi"));
            }
            else
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Şehirler indexlenirken bir hata oluştu"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing cities");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Şehirler indexlenirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// İlçeleri indexler
    /// </summary>
    [HttpPost("index-districts")]
    public async Task<ActionResult<ApiResponse<object>>> IndexDistricts()
    {
        try
        {
            var jsonDataService = new LocationDataService(
                HttpContext.RequestServices.GetRequiredService<ILogger<LocationDataService>>(),
                HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>()
            );

            var districts = await jsonDataService.GetDistrictsAsync();
            var result = await _elasticsearchService.IndexDistrictsAsync(districts);

            if (result)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { count = districts.Count() }, "İlçeler başarıyla indexlendi"));
            }
            else
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("İlçeler indexlenirken bir hata oluştu"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing districts");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("İlçeler indexlenirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Mahalleleri indexler
    /// </summary>
    [HttpPost("index-neighbors")]
    public async Task<ActionResult<ApiResponse<object>>> IndexNeighbors()
    {
        try
        {
            var jsonDataService = new LocationDataService(
                HttpContext.RequestServices.GetRequiredService<ILogger<LocationDataService>>(),
                HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>()
            );

            var neighbors = await jsonDataService.GetNeighborsAsync();
            var result = await _elasticsearchService.IndexNeighborsAsync(neighbors);

            if (result)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { count = neighbors.Count() }, "Mahalleler başarıyla indexlendi"));
            }
            else
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Mahalleler indexlenirken bir hata oluştu"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing neighbors");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Mahalleler indexlenirken bir hata oluştu"));
        }
    }
}


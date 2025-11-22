using ElasticSearchDotNet.Api.Models;
using ElasticSearchDotNet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchDotNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationDataService _locationDataService;
    private readonly ILogger<LocationController> _logger;

    public LocationController(ILocationDataService locationDataService, ILogger<LocationController> logger)
    {
        _locationDataService = locationDataService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm şehirleri getirir
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<ApiResponse<IEnumerable<City>>>> GetCities()
    {
        try
        {
            var cities = await _locationDataService.GetCitiesAsync();
            return Ok(ApiResponse<IEnumerable<City>>.SuccessResponse(cities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cities");
            return StatusCode(500, ApiResponse<IEnumerable<City>>.ErrorResponse("Şehirler yüklenirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Şehir arama
    /// </summary>
    [HttpGet("cities/search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<City>>>> SearchCities([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<IEnumerable<City>>.ErrorResponse("Arama terimi boş olamaz"));
            }

            var cities = await _locationDataService.SearchCitiesAsync(q);
            return Ok(ApiResponse<IEnumerable<City>>.SuccessResponse(cities));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching cities");
            return StatusCode(500, ApiResponse<IEnumerable<City>>.ErrorResponse("Şehir arama sırasında bir hata oluştu"));
        }
    }

    /// <summary>
    /// Kod ile şehir getir
    /// </summary>
    [HttpGet("cities/{code}")]
    public async Task<ActionResult<ApiResponse<City>>> GetCityByCode(string code)
    {
        try
        {
            var city = await _locationDataService.GetCityByCodeAsync(code);
            
            if (city == null)
            {
                return NotFound(ApiResponse<City>.ErrorResponse("Şehir bulunamadı"));
            }

            return Ok(ApiResponse<City>.SuccessResponse(city));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting city by code: {Code}", code);
            return StatusCode(500, ApiResponse<City>.ErrorResponse("Şehir getirilirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Tüm ilçeleri getirir
    /// </summary>
    [HttpGet("districts")]
    public async Task<ActionResult<ApiResponse<IEnumerable<District>>>> GetDistricts([FromQuery] string? cityCode = null)
    {
        try
        {
            IEnumerable<District> districts;
            
            if (!string.IsNullOrWhiteSpace(cityCode))
            {
                districts = await _locationDataService.GetDistrictsByCityCodeAsync(cityCode);
            }
            else
            {
                districts = await _locationDataService.GetDistrictsAsync();
            }

            return Ok(ApiResponse<IEnumerable<District>>.SuccessResponse(districts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting districts");
            return StatusCode(500, ApiResponse<IEnumerable<District>>.ErrorResponse("İlçeler yüklenirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// İlçe arama
    /// </summary>
    [HttpGet("districts/search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<District>>>> SearchDistricts(
        [FromQuery] string q, 
        [FromQuery] string? cityCode = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<IEnumerable<District>>.ErrorResponse("Arama terimi boş olamaz"));
            }

            var districts = await _locationDataService.SearchDistrictsAsync(q, cityCode);
            return Ok(ApiResponse<IEnumerable<District>>.SuccessResponse(districts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching districts");
            return StatusCode(500, ApiResponse<IEnumerable<District>>.ErrorResponse("İlçe arama sırasında bir hata oluştu"));
        }
    }

    /// <summary>
    /// Kod ile ilçe getir
    /// </summary>
    [HttpGet("districts/{code}")]
    public async Task<ActionResult<ApiResponse<District>>> GetDistrictByCode(string code)
    {
        try
        {
            var district = await _locationDataService.GetDistrictByCodeAsync(code);
            
            if (district == null)
            {
                return NotFound(ApiResponse<District>.ErrorResponse("İlçe bulunamadı"));
            }

            return Ok(ApiResponse<District>.SuccessResponse(district));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting district by code: {Code}", code);
            return StatusCode(500, ApiResponse<District>.ErrorResponse("İlçe getirilirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Tüm mahalleleri getirir
    /// </summary>
    [HttpGet("neighbors")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Neighbor>>>> GetNeighbors([FromQuery] string? districtCode = null)
    {
        try
        {
            IEnumerable<Neighbor> neighbors;
            
            if (!string.IsNullOrWhiteSpace(districtCode))
            {
                neighbors = await _locationDataService.GetNeighborsByDistrictCodeAsync(districtCode);
            }
            else
            {
                neighbors = await _locationDataService.GetNeighborsAsync();
            }

            return Ok(ApiResponse<IEnumerable<Neighbor>>.SuccessResponse(neighbors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting neighbors");
            return StatusCode(500, ApiResponse<IEnumerable<Neighbor>>.ErrorResponse("Mahalleler yüklenirken bir hata oluştu"));
        }
    }

    /// <summary>
    /// Mahalle arama
    /// </summary>
    [HttpGet("neighbors/search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<Neighbor>>>> SearchNeighbors(
        [FromQuery] string q, 
        [FromQuery] string? districtCode = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(ApiResponse<IEnumerable<Neighbor>>.ErrorResponse("Arama terimi boş olamaz"));
            }

            var neighbors = await _locationDataService.SearchNeighborsAsync(q, districtCode);
            return Ok(ApiResponse<IEnumerable<Neighbor>>.SuccessResponse(neighbors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching neighbors");
            return StatusCode(500, ApiResponse<IEnumerable<Neighbor>>.ErrorResponse("Mahalle arama sırasında bir hata oluştu"));
        }
    }
}


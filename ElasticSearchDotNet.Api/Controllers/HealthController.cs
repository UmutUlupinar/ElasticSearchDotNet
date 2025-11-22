using ElasticSearchDotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchDotNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var response = new { status = "healthy", timestamp = DateTime.UtcNow };
        return Ok(ApiResponse<object>.SuccessResponse(response, "Service is healthy"));
    }
}


using Microsoft.AspNetCore.Mvc;

namespace Platform.Service.Analytics.Api;

[ApiController]
[Route("api/v1/analytics")]
public class AnalyticsHealthController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult HealthCheck()
    {
        return Ok("Analytics Service is running");
    }
}

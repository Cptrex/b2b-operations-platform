using Microsoft.AspNetCore.Mvc;

namespace Platform.Service.Business.Api;

[ApiController]
[Route("api/v1/business")]
public class BusinessHealthController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult HealthCheck()
    {
        return Ok("Business Service is running");
    }
}

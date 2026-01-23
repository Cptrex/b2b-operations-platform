using Microsoft.AspNetCore.Mvc;

namespace Platform.Service.Notify.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult HealthCheck()
    {
        return Ok("Notify Service is running");
    }
}

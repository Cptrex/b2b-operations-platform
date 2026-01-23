using Microsoft.AspNetCore.Mvc;

namespace Platform.Auth.Business.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult GetHealth()
    {
        return Ok("Ok");
    }
}
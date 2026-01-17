using Microsoft.AspNetCore.Mvc;

namespace Platform.Service.Orders.Api;

[ApiController]
[Route("api/v1/orders")]
public class OrdersHealthController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult HealthCheck()
    {
        return Ok("Orders Service is running");
    }
}

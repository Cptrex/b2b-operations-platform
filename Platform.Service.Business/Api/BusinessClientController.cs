using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Client")]
[Route("api/v1/client/[controller]")]
public class BusinessClientController : ControllerBase
{
    [HttpGet("{businessKey}")]
    public IActionResult GetBusinessByKey(string businessKey)
    {
        return Ok();
    }
}
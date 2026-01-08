using Microsoft.AspNetCore.Mvc;
using Platform.Auth.Business.Dto;

namespace Platform.IdentityService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost]
    public IActionResult Auth(AuthUserDto dto, CancellationToken cancellation) 
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Refresh(RefreshUserDto dto, CancellationToken cancellation) 
    {
        return Ok();
    }
}
using Microsoft.AspNetCore.Mvc;
using Platform.Auth.Business.Api.Dto;

namespace Platform.IdentityService.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("token/auth")]
    public IActionResult Auth(AuthUserDto dto, CancellationToken cancellation) 
    {
        return Ok();
    }

    [HttpPost("token/refresh")]
    public IActionResult Refresh(RefreshUserDto dto, CancellationToken cancellation) 
    {
        return Ok();
    }
}
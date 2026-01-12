using Microsoft.AspNetCore.Mvc;
using Platform.Auth.Business.Api.Dto;
using Platform.Auth.Business.Application;

namespace Platform.Auth.Business.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthorizationService _authService;

    public AuthController(AuthorizationService service)
    {
        _authService = service;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Auth(AuthUserDto dto, CancellationToken cancellation) 
    {
        var result = await _authService.Authorize(dto.Login, dto.Password, dto.BusinessId, cancellation);
        
        if (!result.IsSuccess)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("token/refresh")]
    public IActionResult Refresh(RefreshUserDto dto, CancellationToken cancellation) 
    {
        return Ok();
    }
}
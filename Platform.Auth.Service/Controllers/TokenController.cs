using Microsoft.AspNetCore.Mvc;
using Paltform.Auth.Shared.JwtToken.Results;
using Platform.Auth.Service.Dto;
using Platform.Auth.Service.Services.ServiceToken.Contracts;

namespace Platform.Auth.Service.Controllers;

[ApiController]
[Route("api/v1/internal/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IServiceCredentialStore _store;
    private readonly ITokenIssuer _issuer;
   
    public TokenController(IServiceCredentialStore store, ITokenIssuer issuer)
    {
        _store = store;
        _issuer = issuer;
    }

    [HttpPost]
    public async Task<IActionResult> Issue(TokenRequestDto request, CancellationToken cancellation)
    {
        var valid = await _store.ValidateAsync(request.ServiceId, request.Secret);

        if (!valid)
        {
            return Unauthorized();
        }

        var token = _issuer.ServiceIssue(request.ServiceId);
        var publicKey = _issuer.GetPublicKey();

        return Ok(new AuthResponseDto(
            token.Token,
            token.ExpiresAt,
            publicKey
        ));
    }

    [HttpGet("health-check")]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok("Auth.Service health-check OK");
    }
}
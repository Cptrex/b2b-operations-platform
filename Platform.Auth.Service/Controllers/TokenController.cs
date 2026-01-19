using Microsoft.AspNetCore.Mvc;
using Paltform.Auth.Shared.JwtToken.Contracts;
using Platform.Auth.Service.Dto;
using Platform.Auth.Service.Services.Logging;
using Platform.Auth.Service.Services.ServiceToken.Contracts;
using Platform.Logging.MongoDb;
using Platform.Logging.MongoDb.Contracts;
using Platform.Shared.Results;

namespace Platform.Auth.Service.Controllers;

[ApiController]
[Route("api/v1/internal/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IServiceCredentialStore _store;
    private readonly IServiceTokenIssuer _issuer;
    private readonly ILoggingService _logging;
   
    public TokenController(IServiceCredentialStore store, IServiceTokenIssuer issuer, ILoggingService logging)
    {
        _store = store;
        _issuer = issuer;
        _logging = logging;
    }

    [HttpPost]
    public async Task<IActionResult> Issue(TokenRequestDto request, CancellationToken cancellation)
    {
        var result = await _store.ValidateAsync(request.ServiceId, request.Secret);

        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error?.Message);
        }

        var token = _issuer.ServiceIssue(request.ServiceId);
        var publicKey = _issuer.GetPublicKey();
        var responseDto = new AuthResponseDto(token.Token, token.ExpiresAt, publicKey);

        await _logging.WriteAsync(LogType.Security, LoggingAction.IssueServiceToken, responseDto, cancellation);

        return Ok(Result<AuthResponseDto>.Ok(responseDto));
    }

    [HttpGet("health-check")]
    public async Task<IActionResult> HealthCheck()
    {
        return Ok("Auth.Service health-check OK");
    }
}
using Microsoft.AspNetCore.Mvc;
using Platform.Auth.Business.Api.Dto;
using Platform.Auth.Business.Application;

namespace Platform.Auth.Business.Api;

[ApiController]
[Route("api/v1/internal/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(CreateAccountDto dto, CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateAccountAsync(dto.BusinessId, dto.Login, dto.Name, dto.Email, dto.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount(int accountId, [FromQuery] string businessId, CancellationToken cancellationToken)
    {
        var result = await _accountService.DeleteAccountAsync(accountId, businessId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("business/{businessId}")]
    public async Task<IActionResult> GetAccountsByBusinessId(string businessId, CancellationToken cancellationToken)
    {
        var result = await _accountService.GetAllAccountsByBusinessIdAsync(businessId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

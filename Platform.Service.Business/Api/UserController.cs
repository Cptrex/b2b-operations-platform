using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Service.Business.Api.Dto;
using Platform.Service.Business.Application;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Internal")]
[Route("api/v1/internal/[controller]")]
public class UserController : ControllerBase
{
    private readonly BusinessService _businessService;

    public UserController(BusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto, CancellationToken ct)
    {
        var user = await _businessService.CreateBusinessUserAsync(
            dto.Login,
            Guid.NewGuid(),
            dto.BusinessId,
            ct
        );

        return Ok(new { UserId = user.Id, UserName = user.UserName, BusinessId = user.BusinessId });
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId, [FromQuery] string businessId, CancellationToken ct)
    {
        await _businessService.DeleteUserAsync(userId, businessId, ct);
        return NoContent();
    }
}
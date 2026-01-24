using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Service.Business.Api.Dto;
using Platform.Service.Business.Application;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Client")]
[Route("api/v1/internal/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _uerService;

    public UserController(UserService uerService)
    {
        _uerService = uerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto, CancellationToken ct)
    {
        var result = await _uerService.CreateBusinessUserAsync(dto.Login, Guid.NewGuid(), dto.BusinessId, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId, [FromQuery] string businessId, CancellationToken ct)
    {
        await _uerService.DeleteUserAsync(userId, businessId, ct);

        return NoContent();
    }
}
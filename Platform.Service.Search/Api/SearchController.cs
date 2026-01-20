using Microsoft.AspNetCore.Mvc;
using Platform.Service.Search.Application;

namespace Platform.Service.Search.Api;

[ApiController]
[Route("api/v1/search")]
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("health-check")]
    public IActionResult HealthCheck()
    {
        return Ok("Search Service is running");
    }

    [HttpGet("business")]
    public async Task<IActionResult> SearchBusiness([FromQuery] string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Business name is required");
        }

        var businesses = await _searchService.SearchBusinessByNameAsync(name, ct);

        return Ok(businesses);
    }

    [HttpGet("user")]
    public async Task<IActionResult> SearchUser([FromQuery] string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("User name is required");
        }

        var users = await _searchService.SearchUserByNameAsync(name, ct);

        return Ok(users);
    }

    [HttpGet("account/login")]
    public async Task<IActionResult> SearchAccountByLogin([FromQuery] string login, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return BadRequest("Login is required");
        }

        var accounts = await _searchService.SearchAccountByLoginAsync(login, ct);

        return Ok(accounts);
    }

    [HttpGet("account/email")]
    public async Task<IActionResult> SearchAccountByEmail([FromQuery] string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required");
        }

        var accounts = await _searchService.SearchAccountByEmailAsync(email, ct);

        return Ok(accounts);
    }

    [HttpGet("account/name")]
    public async Task<IActionResult> SearchAccountByName([FromQuery] string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Name is required");
        }

        var accounts = await _searchService.SearchAccountByNameAsync(name, ct);

        return Ok(accounts);
    }
}
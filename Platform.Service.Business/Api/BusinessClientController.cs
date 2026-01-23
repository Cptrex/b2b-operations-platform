using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Service.Business.Api.Dto;
using Platform.Service.Business.Application;

namespace Platform.Service.Business.Api;

[ApiController]
[Route("api/v1/client/business")]
public class BusinessClientController : ControllerBase
{
    private readonly BusinessService _businessService;

    public BusinessClientController(BusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet("{businessId}")]
    public IActionResult GetBusinessById(string businessId)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBusiness(CreateBusinessDto dto, CancellationToken ct)
    {
        var result = await _businessService.CreateBusinessAsync(dto.BusinessName, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpDelete("{businessId}")]
    public async Task<IActionResult> DeleteBusiness(string businessId, CancellationToken ct)
    {
        await _businessService.DeleteBusinessAsync(businessId, ct);

        return NoContent();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Service.Business.Api.Dto;
using Platform.Service.Business.Application;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Internal")]
[Route("api/v1/internal/business")]
public class BusinessInternalController : ControllerBase
{
    private readonly BusinessService _businessService;

    public BusinessInternalController(BusinessService businessService)
    {
        _businessService = businessService;
    }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness(CreateBusinessDto dto, CancellationToken ct)
        {
            var business = await _businessService.CreateBusinessAsync(dto.BusinessId, dto.BusinessName, ct);

            return Ok(new { BusinessId = business.BusinessId, BusinessName = business.BusinessName });
        }

        [HttpDelete("{businessId}")]
        public async Task<IActionResult> DeleteBusiness(string businessId, CancellationToken ct)
        {
            await _businessService.DeleteBusinessAsync(businessId, ct);

            return NoContent();
        }
    }

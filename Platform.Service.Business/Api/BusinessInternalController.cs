using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Internal")]
[Route("api/v1/internal/business")]
public class BusinessInternalController : ControllerBase
{
}
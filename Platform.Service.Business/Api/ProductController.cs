using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Service.Business.Api.Dto;
using Platform.Service.Business.Application;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Internal")]
[Route("api/v1/internal/business/{businessId}/products")]
public class ProductController : ControllerBase
{
    private readonly BusinessService _businessService;

    public ProductController(BusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(string businessId, AddProductDto dto, CancellationToken ct)
    {
        var product = await _businessService.AddProductToCatalogAsync(businessId, dto.ProductName, dto.Description, dto.Price, ct);

        return Ok(new
        {
            ProductId = product.ProductId,
            BusinessId = product.BusinessId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            IsAvailable = product.IsAvailable,
            CreatedAt = product.CreatedAt
        });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveProduct(string businessId, Guid productId, CancellationToken ct)
    {
        var product = await _businessService.RemoveProductFromCatalogAsync(productId, ct);

        return NoContent();
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(string businessId, Guid productId, UpdateProductDto dto, CancellationToken ct)
    {
        var product = await _businessService.UpdateProductInfoAsync(productId, dto.ProductName, dto.Description, dto.Price, ct);

        return Ok(new
        {
            ProductId = product.ProductId,
            BusinessId = product.BusinessId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            UpdatedAt = product.UpdatedAt
        });
    }

    [HttpPatch("{productId}/availability")]
    public async Task<IActionResult> SetProductAvailability(string businessId, Guid productId, SetProductAvailabilityDto dto, CancellationToken ct)
    {
        var product = await _businessService.SetProductAvailabilityAsync(productId, dto.IsAvailable, ct);

        return Ok(new
        {
            ProductId = product.ProductId,
            BusinessId = product.BusinessId,
            IsAvailable = product.IsAvailable
        });
    }
}

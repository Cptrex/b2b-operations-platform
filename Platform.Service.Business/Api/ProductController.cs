using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Service.Business.Api.Dto;
using Platform.Service.Business.Application;

namespace Platform.Service.Business.Api;

[ApiController]
[Authorize(Policy = "Client")]
[Route("api/v1/internal/business/{businessId}/products")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(string businessId, AddProductDto dto, CancellationToken ct)
    {
        var result = await _productService.AddProductToCatalogAsync(businessId, dto.ProductName, dto.Description, dto.Price, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveProduct(string businessId, Guid productId, CancellationToken ct)
    {
        await _productService.RemoveProductFromCatalogAsync(productId, ct);

        return NoContent();
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(string businessId, Guid productId, UpdateProductDto dto, CancellationToken ct)
    {
        var result = await _productService.UpdateProductInfoAsync(productId, dto.ProductName, dto.Description, dto.Price, ct);

        if (result.IsSuccess == false) 
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpPatch("{productId}/availability")]
    public async Task<IActionResult> SetProductAvailability(string businessId, Guid productId, SetProductAvailabilityDto dto, CancellationToken ct)
    {
        var result = await _productService.SetProductAvailabilityAsync(productId, dto.IsAvailable, ct);

        if (result.IsSuccess == false) 
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
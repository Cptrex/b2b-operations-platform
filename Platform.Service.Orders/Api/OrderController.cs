using Microsoft.AspNetCore.Mvc;
using Platform.Service.Orders.Api.Dto;
using Platform.Service.Orders.Application;
using Platform.Service.Orders.Domain.Order;

namespace Platform.Service.Orders.Api;

[ApiController]
[Route("api/v1/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _orderService.CreateOrderAsync(dto.BusinessId, dto.CustomerId, dto.CustomerName, dto.CustomerEmail, dto.CustomerPhone, dto.Items, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpPost("{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId, CancellationToken ct)
    {
        var result = await _orderService.ConfirmOrderAsync(orderId, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken ct)
    {
        var result = await _orderService.CancelOrderAsync(orderId, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpPut("{orderId}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(Guid orderId, UpdatePaymentStatusDto dto, CancellationToken ct)
    {
        var paymentStatus = (PaymentStatus)dto.PaymentStatus;

        var result = await _orderService.UpdatePaymentStatusAsync(orderId, paymentStatus, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }

    [HttpPut("{orderId}/delivery-status")]
    public async Task<IActionResult> UpdateDeliveryStatus(Guid orderId, UpdateDeliveryStatusDto dto, CancellationToken ct)
    {
        var deliveryStatus = (DeliveryStatus)dto.DeliveryStatus;

        var result = await _orderService.UpdateDeliveryStatusAsync(orderId, deliveryStatus, ct);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Error!.Message);
        }

        return Ok(result);
    }
}
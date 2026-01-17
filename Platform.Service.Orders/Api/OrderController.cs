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
        var items = dto.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Price = i.Price,
            Quantity = i.Quantity
        }).ToList();

        var order = await _orderService.CreateOrderAsync(
            dto.BusinessId,
            dto.CustomerId,
            dto.CustomerName,
            dto.CustomerEmail,
            dto.CustomerPhone,
            items,
            ct);

        return Ok(new
        {
            OrderId = order.OrderId,
            BusinessId = order.BusinessId,
            CustomerId = order.CustomerId,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt
        });
    }

    [HttpPost("{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId, CancellationToken ct)
    {
        var order = await _orderService.ConfirmOrderAsync(orderId, ct);

        return Ok(new
        {
            OrderId = order.OrderId,
            Status = order.Status.ToString(),
            ConfirmedAt = order.ConfirmedAt
        });
    }

    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken ct)
    {
        var order = await _orderService.CancelOrderAsync(orderId, ct);

        return Ok(new
        {
            OrderId = order.OrderId,
            Status = order.Status.ToString(),
            CancelledAt = order.CancelledAt
        });
    }

    [HttpPut("{orderId}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(Guid orderId, UpdatePaymentStatusDto dto, CancellationToken ct)
    {
        var paymentStatus = (PaymentStatus)dto.PaymentStatus;
        var order = await _orderService.UpdatePaymentStatusAsync(orderId, paymentStatus, ct);

        return Ok(new
        {
            OrderId = order.OrderId,
            PaymentStatus = order.PaymentStatus.ToString()
        });
    }

    [HttpPut("{orderId}/delivery-status")]
    public async Task<IActionResult> UpdateDeliveryStatus(Guid orderId, UpdateDeliveryStatusDto dto, CancellationToken ct)
    {
        var deliveryStatus = (DeliveryStatus)dto.DeliveryStatus;
        var order = await _orderService.UpdateDeliveryStatusAsync(orderId, deliveryStatus, ct);

        return Ok(new
        {
            OrderId = order.OrderId,
            DeliveryStatus = order.DeliveryStatus.ToString(),
            Status = order.Status.ToString()
        });
    }

    [HttpGet("health-check")]
    public IActionResult HealthCheck()
    {
        return Ok("Orders Service is running");
    }
}

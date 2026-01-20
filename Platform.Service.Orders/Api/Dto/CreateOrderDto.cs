namespace Platform.Service.Orders.Api.Dto;

public sealed record CreateOrderDto
(
    string BusinessId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    List<CreateOrderItemDto> Items
);

namespace Platform.Service.Orders.Api.Dto;

public sealed record CreateOrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity
);
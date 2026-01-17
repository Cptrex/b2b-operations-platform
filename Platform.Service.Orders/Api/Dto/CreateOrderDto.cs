namespace Platform.Service.Orders.Api.Dto;

public class CreateOrderDto
{
    public string BusinessId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public List<CreateOrderItemDto> Items { get; set; }

    public CreateOrderDto()
    {
        BusinessId = string.Empty;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        CustomerPhone = string.Empty;
        Items = [];
    }
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public CreateOrderItemDto()
    {
        ProductName = string.Empty;
    }
}

namespace Platform.Service.Orders.Domain.Order;

public class OrderItem
{
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Order Order { get; set; }

    private OrderItem()
    {
        ProductName = string.Empty;
        Order = null!;
    }

    public OrderItem(Guid productId, string productName, decimal price, int quantity)
    {
        OrderItemId = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
        Order = null!;
    }
}

namespace Platform.Service.Orders.Domain.Order;

public class Order
{
    public Guid OrderId { get; set; }
    public string BusinessId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DeliveryStatus DeliveryStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public long CreatedAt { get; set; }
    public long? ConfirmedAt { get; set; }
    public long? CancelledAt { get; set; }

    private List<OrderItem> _items = [];
    public List<OrderItem> Items 
    { 
        get 
        { 
            return _items; 
        } 
        set 
        { 
            if (value == null)
            {
                _items = [];
            }
            else
            {
                _items = value;
            }
        } 
    }

    private Order()
    {
        BusinessId = string.Empty;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        CustomerPhone = string.Empty;
        _items = [];
    }

    public Order(string businessId, Guid customerId, string customerName, string customerEmail, string customerPhone)
    {
        OrderId = Guid.NewGuid();
        BusinessId = businessId;
        CustomerId = customerId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
        Status = OrderStatus.Pending;
        PaymentStatus = PaymentStatus.Pending;
        DeliveryStatus = DeliveryStatus.Pending;
        TotalAmount = 0;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _items = [];
    }

    public void AddItem(OrderItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _items.Add(item);
        RecalculateTotalAmount();
    }

    public void RemoveItem(OrderItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _items.Remove(item);
        RecalculateTotalAmount();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot confirm order with status {Status}");
        }

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Order is already cancelled");
        }

        if (Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Cannot cancel delivered order");
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void UpdatePaymentStatus(PaymentStatus newStatus)
    {
        PaymentStatus = newStatus;
    }

    public void UpdateDeliveryStatus(DeliveryStatus newStatus)
    {
        if (newStatus == DeliveryStatus.Delivered)
        {
            Status = OrderStatus.Delivered;
        }

        DeliveryStatus = newStatus;
    }

    private void RecalculateTotalAmount()
    {
        decimal total = 0;
        foreach (var item in _items)
        {
            total += item.Price * item.Quantity;
        }
        TotalAmount = total;
    }
}

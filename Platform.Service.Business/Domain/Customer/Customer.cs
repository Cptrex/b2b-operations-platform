namespace Platform.Service.Business.Domain.Customer;

public class Customer
{
    public Guid CustomerId { get; set; }
    public string BusinessId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public long CreatedAt { get; set; }

    public Business.Business Business { get; set; }

    private Customer()
    {
        BusinessId = string.Empty;
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        CustomerPhone = string.Empty;
        Business = null!;
    }

    public Customer(Guid customerId, string businessId, string customerName, string customerEmail, string customerPhone)
    {
        CustomerId = customerId;
        BusinessId = businessId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Business = null!;
    }
}

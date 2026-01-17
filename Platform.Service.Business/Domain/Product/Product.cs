namespace Platform.Service.Business.Domain.Product;

public class Product
{
    public Guid ProductId { get; set; }
    public string BusinessId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public long CreatedAt { get; set; }
    public long? UpdatedAt { get; set; }

    public Business.Business Business { get; set; }

    private Product()
    {
        BusinessId = string.Empty;
        ProductName = string.Empty;
        Description = string.Empty;
        Business = null!;
    }

    public Product(string businessId, string productName, string description, decimal price)
    {
        ProductId = Guid.NewGuid();
        BusinessId = businessId;
        ProductName = productName;
        Description = description;
        Price = price;
        IsAvailable = true;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Business = null!;
    }

    public void UpdateInfo(string productName, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentNullException(nameof(productName));
        }

        ProductName = productName;
        Description = description;
        Price = price;
        UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

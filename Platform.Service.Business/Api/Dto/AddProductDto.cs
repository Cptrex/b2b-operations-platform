namespace Platform.Service.Business.Api.Dto;

public class AddProductDto
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public AddProductDto()
    {
        ProductName = string.Empty;
        Description = string.Empty;
    }
}

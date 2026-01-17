namespace Platform.Service.Business.Api.Dto;

public class UpdateProductDto
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public UpdateProductDto()
    {
        ProductName = string.Empty;
        Description = string.Empty;
    }
}

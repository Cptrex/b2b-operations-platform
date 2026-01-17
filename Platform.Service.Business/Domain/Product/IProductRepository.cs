namespace Platform.Service.Business.Domain.Product;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(Guid productId);
    Task<List<Product>> GetProductsByBusinessIdAsync(string businessId);
    Task CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    Task Save();
}

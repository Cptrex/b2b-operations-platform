using Microsoft.EntityFrameworkCore;
using Platform.Service.Business.Domain.Product;

namespace Platform.Service.Business.Infrastructure.Db;

public class ProductRepository : IProductRepository
{
    private readonly BusinessContext _context;

    public ProductRepository(BusinessContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
    }

    public async Task<List<Product>> GetProductsByBusinessIdAsync(string businessId)
    {
        return await _context.Products.Where(p => p.BusinessId == businessId).ToListAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteProductAsync(Product product)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Platform.Service.Business.Domain.Customer;

namespace Platform.Service.Business.Infrastructure.Db;

public class CustomerRepository : ICustomerRepository
{
    private readonly BusinessContext _context;

    public CustomerRepository(BusinessContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid customerId, string businessId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.BusinessId == businessId);
    }

    public async Task<List<Customer>> GetCustomersByBusinessIdAsync(string businessId)
    {
        return await _context.Customers.Where(c => c.BusinessId == businessId).ToListAsync();
    }

    public async Task CreateCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public Task UpdateCustomerAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public Task DeleteCustomerAsync(Customer customer)
    {
        _context.Customers.Remove(customer);
        return Task.CompletedTask;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}

namespace Platform.Service.Business.Domain.Customer;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(Guid customerId, string businessId);
    Task<List<Customer>> GetCustomersByBusinessIdAsync(string businessId);
    Task CreateCustomerAsync(Customer customer);
    Task UpdateCustomerAsync(Customer customer);
    Task DeleteCustomerAsync(Customer customer);
    Task Save();
}

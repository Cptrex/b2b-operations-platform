namespace Platform.Service.Business.Domain.Business;

public interface IBusinessRepository
{
    Task<Business?> GetByBusinessByIdAsync(string businessId);
    Task<Business?> GetByBusinessNameAsync(string businessName);
    Task DeleteBusinessByIdAsync(string businessId);
    Task AddBusinessAsync(Business business);
    Task<Domain.Business.Business> CreateBusinessAsync(Domain.Business.Business business);
    Task DeleteBusinessAsync(Business business);
    Task UpdateBusinessAsync(Business business);
    Task Save();
}
namespace Platform.Service.Business.Domain.Business;

public interface IBusinessRepository
{
    Task<Business?> GetByBusinessByIdAsync(string businessId);
    Task DeleteBusinessByIdAsync(string businessId);
    Task AddBusinessAsync(Business business);
    Task CreateBusinessAsync(Business business);
    Task DeleteBusinessAsync(Business business);
    Task UpdateBusinessAsync(Business business);
    Task Save();
}
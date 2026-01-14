namespace Platform.Service.Business.Domain.Business;

public interface IBusinessRepository
{
    Task<Business?> GetByBusinessByIdAsync(string businessKey);
    Task DeleteBusinessByKeyAsync(string businessKey);
    Task AddBusinessAsync(Business business);
    Task CreateBusinessAsync(Business business);
    Task DeleteBusinessAsync(Business business);
    Task UpdateBusinessAsync(Business business);
    Task Save();
}
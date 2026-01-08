namespace Platform.Service.Business.Domain.Business;

public interface IBusinessRepository
{
    Task<Business> GetByBusinessKeyAsync(string businessKey);
    Task DeleteBusinessByKeyAsync(string businessKey);
    Task AddBusinessAsync(Business business);
    Task UpdateBusinessAsync(Business business);
    Task Save();
}
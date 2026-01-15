namespace Platform.Service.Search.Domain.Business;

public interface IBusinessRepository
{
    Task<Business?> GetByBusinessIdAsync(string businessId);
    Task<List<Business>> SearchByNameAsync(string businessName, CancellationToken ct = default);
    Task AddBusinessAsync(Business business);
    Task Save();
}

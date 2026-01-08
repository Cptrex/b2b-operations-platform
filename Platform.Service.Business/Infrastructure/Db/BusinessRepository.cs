using Platform.Service.Business.Domain.Business;

namespace Platform.Service.Business.Infrastructure.Db;

public class BusinessRepository : IBusinessRepository
{
    public Task AddBusinessAsync(Domain.Business.Business business)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Business.Business> GetByBusinessKeyAsync(string businessKey)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBusinessAsync(Domain.Business.Business business)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBusinessByKeyAsync(string businessKey)
    {
        throw new NotImplementedException();
    }

    public Task Save()
    {
        throw new NotImplementedException();
    }
}
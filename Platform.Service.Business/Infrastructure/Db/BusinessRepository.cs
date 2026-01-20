using Microsoft.EntityFrameworkCore;
using Platform.Service.Business.Domain.Business;

namespace Platform.Service.Business.Infrastructure.Db;

public class BusinessRepository : IBusinessRepository
{
    private readonly BusinessContext _context;

    public BusinessRepository(BusinessContext context)
    {
        _context = context;
    }

    public async Task<Domain.Business.Business?> GetByBusinessByIdAsync(string businessKey)
    {
        return await _context.Businesses.Include(b => b.Users).FirstOrDefaultAsync(b => b.BusinessId == businessKey);
    }

    public async Task AddBusinessAsync(Domain.Business.Business business)
    {
        await _context.Businesses.AddAsync(business);
    }

    public async Task CreateBusinessAsync(Domain.Business.Business business)
    {
        await _context.Businesses.AddAsync(business);
    }

    public Task DeleteBusinessAsync(Domain.Business.Business business)
    {
        _context.Businesses.Remove(business);
        return Task.CompletedTask;
    }

    public Task UpdateBusinessAsync(Domain.Business.Business business)
    {
        _context.Businesses.Update(business);
        return Task.CompletedTask;
    }

    public async Task DeleteBusinessByIdAsync(string businessKey)
    {
        var business = await GetByBusinessByIdAsync(businessKey);

        if (business != null)
        {
            _context.Businesses.Remove(business);
        }
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
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

    public async Task<Domain.Business.Business> CreateBusinessAsync(Domain.Business.Business business)
    {
        var newEntity = await _context.Businesses.AddAsync(business);

        return newEntity.Entity;
    }

    public async Task DeleteBusinessAsync(Domain.Business.Business business)
    {
        await _context.Businesses.Remove(business);
    }

    public async Task UpdateBusinessAsync(Domain.Business.Business business)
    {
        await _context.Businesses.ExecuteUpdateAsync(business);
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

    public async Task<Domain.Business.Business?> GetByBusinessNameAsync(string businessName)
    {
        return await _context.Businesses.FirstOrDefault(b => b.BusinessName == businessName);
    }
}
using Microsoft.EntityFrameworkCore;
using Platform.Service.Search.Domain.Business;

namespace Platform.Service.Search.Infrastructure.Db;

public class BusinessRepository : IBusinessRepository
{
    private readonly SearchContext _context;

    public BusinessRepository(SearchContext context)
    {
        _context = context;
    }

    public async Task<Domain.Business.Business?> GetByBusinessIdAsync(string businessId)
    {
        return await _context.Businesses.FirstOrDefaultAsync(b => b.BusinessId == businessId);
    }

    public async Task<List<Domain.Business.Business>> SearchByNameAsync(string businessName, CancellationToken ct = default)
    {
        return await _context.Businesses.Where(b => b.BusinessName.Contains(businessName)).ToListAsync(ct);
    }

    public async Task AddBusinessAsync(Domain.Business.Business business)
    {
        await _context.Businesses.AddAsync(business);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}

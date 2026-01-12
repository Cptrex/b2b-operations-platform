using Microsoft.EntityFrameworkCore;

namespace Platform.Service.Business.Infrastructure.Db;

public class BusinessContext : DbContext
{
    public DbSet<Domain.Business.Business> Businesses { get; set; } = null!;

    public BusinessContext(DbContextOptions<BusinessContext> options) : base(options)
    {
    }
}

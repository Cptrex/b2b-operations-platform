using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Service.Business.Infrastructure.Db;

public class BusinessContextFactory : IDesignTimeDbContextFactory<BusinessContext>
{
    public BusinessContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BusinessContext>();
        
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=platform_business_db;Username=postgres;Password=postgres");

        return new BusinessContext(optionsBuilder.Options);
    }
}

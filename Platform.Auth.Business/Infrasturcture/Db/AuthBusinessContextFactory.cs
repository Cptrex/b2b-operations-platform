using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AuthBusinessContextFactory : IDesignTimeDbContextFactory<AuthBusinessContext>
{
    public AuthBusinessContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthBusinessContext>();
        
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=platform_auth_business_db;Username=postgres;Password=postgres");

        return new AuthBusinessContext(optionsBuilder.Options);
    }
}

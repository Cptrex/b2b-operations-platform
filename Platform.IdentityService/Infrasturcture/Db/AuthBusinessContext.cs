using Microsoft.EntityFrameworkCore;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AuthBusinessContext : DbContext
{
    public DbSet<Domain.Account.Account> Accounts { get; set; } = null!;

    public AuthBusinessContext(DbContextOptions<AuthBusinessContext> options) : base(options)
    {
    }
}
using Microsoft.EntityFrameworkCore;
using Platform.Auth.Business.Domain.Account.ValueObjects;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AuthBusinessContext : DbContext
{
    public DbSet<Domain.Account.Account> Accounts { get; set; } = null!;

    public AuthBusinessContext(DbContextOptions<AuthBusinessContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var table = modelBuilder.Entity<Domain.Account.Account>().ToTable("accounts");
        
        table.HasKey(e => e.Id);

        table.Property(e => e.Id).HasColumnName("id");
        table.Property(e => e.BusinessId).HasColumnName("business_id");
        table.Property(e => e.Login).HasColumnName("login");
        table.Property(e => e.Name).HasColumnName("name");
        table.Property(e => e.Email)
             .HasColumnName("email")
             .HasConversion(
                 email => email.Value,
                 value => Email.Create(value)
              );
        table.Property(e => e.Password)
            .HasColumnName("password")
            .HasConversion(
                password => password.Hash,
                value => PasswordHash.Create(value)
             );
    }
}
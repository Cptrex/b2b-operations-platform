using Microsoft.EntityFrameworkCore;
using Platform.Auth.Business.Domain.Account.ValueObjects;
using Platform.Auth.Business.Infrasturcture.Db.Entity;

namespace Platform.Auth.Business.Infrasturcture.Db;

public class AuthBusinessContext : DbContext
{
    public DbSet<Domain.Account.Account> Accounts { get; set; } = null!;
    public DbSet<Entity.OutboxMessage> OutboxMessages { get; set; } = null!;

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

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();
            entity.Property(e => e.EventId)
                .HasColumnName("event_id")
                .IsRequired();
            entity.HasIndex(e => e.EventId)
                .IsUnique();
            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(e => e.RoutingKey)
                .HasColumnName("routing_key")
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(e => e.Payload)
                .HasColumnName("payload")
                .IsRequired();
            entity.Property(e => e.OccurredAt)
                .HasColumnName("occurred_at")
                .IsRequired();
            entity.Property(e => e.PublishedAt)
                .HasColumnName("published_at");
            entity.Property(e => e.RetryCount)
                .HasColumnName("retry_count")
                .HasDefaultValue(0)
                .IsRequired();
            entity.Property(e => e.LastError)
                .HasColumnName("last_error")
                .HasMaxLength(2000);

            entity.HasIndex(e => new { e.PublishedAt, e.OccurredAt });
        });
    }
}
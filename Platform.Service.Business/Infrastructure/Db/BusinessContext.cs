using Microsoft.EntityFrameworkCore;
using Platform.Service.Business.Infrastructure.Db.Entity;

namespace Platform.Service.Business.Infrastructure.Db;

public class BusinessContext : DbContext
{
    public DbSet<Domain.Business.Business> Businesses { get; set; } = null!;
    public DbSet<Domain.User.User> Users { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;

    public BusinessContext(DbContextOptions<BusinessContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Business.Business>(entity =>
        {
            entity.ToTable("businesses");

            entity.HasKey(e => e.BusinessId);

            entity.Property(e => e.BusinessId)
                .HasColumnName("business_id")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.BusinessName)
                .HasColumnName("business_name")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.HasMany(e => e.Users)
                  .WithOne(u => u.Business)
                  .HasForeignKey(u => u.BusinessId)
                  .HasPrincipalKey(b => b.BusinessId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Domain.User.User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            entity.Property(e => e.UserName)
                .HasColumnName("username")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.BusinessId)
                .HasColumnName("business_id")
                .HasMaxLength(36)
                .IsRequired();
        });

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

        modelBuilder.Entity<InboxMessage>(entity =>
        {
            entity.ToTable("inbox_messages");

            entity.HasKey(x => new { x.EventId, x.Consumer });

            entity.Property(e => e.EventId)
                .HasColumnName("event_id")
                .IsRequired();
            entity.Property(e => e.ProcessedAt)
                .HasColumnName("processed_at")
                .IsRequired();
            entity.Property(e => e.Consumer)
                .HasColumnName("consumer")
                .IsRequired();
        });
    }
}
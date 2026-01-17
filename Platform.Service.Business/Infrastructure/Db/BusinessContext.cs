using Microsoft.EntityFrameworkCore;
using Platform.Service.Business.Domain.Customer;
using Platform.Service.Business.Domain.Product;
using Platform.Service.Business.Domain.User;
using Platform.Service.Business.Infrastructure.Db.Entity;

namespace Platform.Service.Business.Infrastructure.Db;

public class BusinessContext : DbContext
{
    public DbSet<Domain.Business.Business> Businesses { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
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

            entity.HasMany(e => e.Products)
                  .WithOne(p => p.Business)
                  .HasForeignKey(p => p.BusinessId)
                  .HasPrincipalKey(b => b.BusinessId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Customers)
                  .WithOne(c => c.Business)
                  .HasForeignKey(c => c.BusinessId)
                  .HasPrincipalKey(b => b.BusinessId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
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

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            entity.Property(e => e.BusinessId)
                .HasColumnName("business_id")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.IsAvailable)
                .HasColumnName("is_available")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            entity.HasKey(e => new { e.CustomerId, e.BusinessId });

            entity.Property(e => e.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            entity.Property(e => e.BusinessId)
                .HasColumnName("business_id")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.CustomerName)
                .HasColumnName("customer_name")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.CustomerEmail)
                .HasColumnName("customer_email")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.CustomerPhone)
                .HasColumnName("customer_phone")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
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
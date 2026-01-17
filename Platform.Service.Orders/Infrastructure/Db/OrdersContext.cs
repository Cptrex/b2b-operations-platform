using Microsoft.EntityFrameworkCore;
using Platform.Service.Orders.Infrastructure.Db.Entity;
using Platform.Service.Orders.Domain.Order;

namespace Platform.Service.Orders.Infrastructure.Db;

public class OrdersContext : DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxMessage> InboxMessages { get; set; } = null!;

    public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            entity.HasKey(e => e.OrderId);

            entity.Property(e => e.OrderId)
                .HasColumnName("order_id")
                .IsRequired();

            entity.Property(e => e.BusinessId)
                .HasColumnName("business_id")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.CustomerId)
                .HasColumnName("customer_id")
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

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.PaymentStatus)
                .HasColumnName("payment_status")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.DeliveryStatus)
                .HasColumnName("delivery_status")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.TotalAmount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(e => e.ConfirmedAt)
                .HasColumnName("confirmed_at");

            entity.Property(e => e.CancelledAt)
                .HasColumnName("cancelled_at");

            entity.HasMany(e => e.Items)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .HasPrincipalKey(o => o.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");

            entity.HasKey(e => e.OrderItemId);

            entity.Property(e => e.OrderItemId)
                .HasColumnName("order_item_id")
                .IsRequired();

            entity.Property(e => e.OrderId)
                .HasColumnName("order_id")
                .IsRequired();

            entity.Property(e => e.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            entity.Property(e => e.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity")
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
        });

        modelBuilder.Entity<InboxMessage>(entity =>
        {
            entity.ToTable("inbox_messages");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(e => e.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            entity.HasIndex(e => e.EventId)
                .IsUnique();

            entity.Property(e => e.ProcessedAt)
                .HasColumnName("processed_at")
                .IsRequired();

            entity.Property(e => e.Consumer)
                .HasColumnName("consumer")
                .HasMaxLength(200)
                .IsRequired();
        });
    }
}

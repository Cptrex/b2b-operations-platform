using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Service.Orders.Infrastructure.Db;

public class OrdersContextFactory : IDesignTimeDbContextFactory<OrdersContext>
{
    public OrdersContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=orders_db;Username=postgres;Password=postgres");

        return new OrdersContext(optionsBuilder.Options);
    }
}

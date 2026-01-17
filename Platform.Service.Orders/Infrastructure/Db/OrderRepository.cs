using Microsoft.EntityFrameworkCore;
using Platform.Service.Orders.Domain.Order;

namespace Platform.Service.Orders.Infrastructure.Db;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersContext _context;

    public OrderRepository(OrdersContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId)
    {
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<List<Order>> GetOrdersByBusinessIdAsync(string businessId)
    {
        return await _context.Orders.Include(o => o.Items).Where(o => o.BusinessId == businessId).ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
    {
        return await _context.Orders.Include(o => o.Items).Where(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public Task UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public Task DeleteOrderAsync(Order order)
    {
        _context.Orders.Remove(order);
        return Task.CompletedTask;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}

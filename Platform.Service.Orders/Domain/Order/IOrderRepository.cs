namespace Platform.Service.Orders.Domain.Order;

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(Guid orderId);
    Task<List<Order>> GetOrdersByBusinessIdAsync(string businessId);
    Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
    Task CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Order order);
    Task Save();
}

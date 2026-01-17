using Microsoft.EntityFrameworkCore;
using Platform.Service.Orders.Infrastructure.Db;
using Platform.Shared.Messaging.Contracts;

namespace Platform.Service.Orders.Infrastructure.Messaging;

public class OrdersRabbitMqConsumer : IRabbitMqMessageConsumer
{
    private readonly OrdersContext _ordersContext;

    public OrdersRabbitMqConsumer(OrdersContext ordersContext)
    {
        _ordersContext = ordersContext;
    }

    public async Task HandleAsync(string routingKey, string body, CancellationToken ct)
    {
        await Task.CompletedTask;
    }
}

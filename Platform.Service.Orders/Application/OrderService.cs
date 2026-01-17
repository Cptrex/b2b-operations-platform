using Platform.Service.Orders.Domain.Order;
using Platform.Service.Orders.Infrastructure.Db;
using Platform.Service.Orders.Infrastructure.Db.Entity;
using Platform.Shared.Messaging.Contracts.Events.Orders;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Platform.Service.Orders.Application;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrdersContext _context;

    public OrderService(IOrderRepository orderRepository, OrdersContext context)
    {
        _orderRepository = orderRepository;
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(string businessId, Guid customerId, string customerName, string customerEmail, string customerPhone, List<OrderItemDto> items, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(businessId))
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("CustomerId cannot be empty", nameof(customerId));
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentNullException(nameof(customerName));
        }

        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            throw new ArgumentNullException(nameof(customerEmail));
        }

        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one item", nameof(items));
        }

        var order = new Order(businessId, customerId, customerName, customerEmail, customerPhone);

        foreach (var itemDto in items)
        {
            var orderItem = new OrderItem(itemDto.ProductId, itemDto.ProductName, itemDto.Price, itemDto.Quantity);
            order.AddItem(orderItem);
        }

        await _orderRepository.CreateOrderAsync(order);

        var orderCreatedEvent = new OrderCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            OrderId = order.OrderId,
            BusinessId = order.BusinessId,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhone = order.CustomerPhone,
            TotalAmount = order.TotalAmount,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(order.CreatedAt)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = orderCreatedEvent.EventId,
            Type = nameof(OrderCreatedEvent),
            RoutingKey = "orders.orderCreated",
            Payload = JsonSerializer.Serialize(orderCreatedEvent),
            OccurredAt = orderCreatedEvent.OccuredAt
        }, ct);

        var customerAddedEvent = new CustomerAddedToBusinessEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            BusinessId = order.BusinessId,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            CustomerPhone = order.CustomerPhone
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = customerAddedEvent.EventId,
            Type = nameof(CustomerAddedToBusinessEvent),
            RoutingKey = "orders.customerAddedToBusiness",
            Payload = JsonSerializer.Serialize(customerAddedEvent),
            OccurredAt = customerAddedEvent.OccuredAt
        }, ct);

        await _orderRepository.Save();

        return order;
    }

    public async Task<Order> ConfirmOrderAsync(Guid orderId, CancellationToken ct)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));
        }

        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with id '{orderId}' not found");
        }

        order.Confirm();

        await _orderRepository.UpdateOrderAsync(order);

        var orderConfirmedEvent = new OrderConfirmedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            OrderId = order.OrderId,
            ConfirmedAt = DateTimeOffset.FromUnixTimeSeconds(order.ConfirmedAt ?? 0)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = orderConfirmedEvent.EventId,
            Type = nameof(OrderConfirmedEvent),
            RoutingKey = "orders.orderConfirmed",
            Payload = JsonSerializer.Serialize(orderConfirmedEvent),
            OccurredAt = orderConfirmedEvent.OccuredAt
        }, ct);

        await _orderRepository.Save();

        return order;
    }

    public async Task<Order> CancelOrderAsync(Guid orderId, CancellationToken ct)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));
        }

        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with id '{orderId}' not found");
        }

        order.Cancel();

        await _orderRepository.UpdateOrderAsync(order);

        var orderCancelledEvent = new OrderCancelledEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            OrderId = order.OrderId,
            CancelledAt = DateTimeOffset.FromUnixTimeSeconds(order.CancelledAt ?? 0)
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = orderCancelledEvent.EventId,
            Type = nameof(OrderCancelledEvent),
            RoutingKey = "orders.orderCancelled",
            Payload = JsonSerializer.Serialize(orderCancelledEvent),
            OccurredAt = orderCancelledEvent.OccuredAt
        }, ct);

        await _orderRepository.Save();

        return order;
    }

    public async Task<Order> UpdatePaymentStatusAsync(Guid orderId, PaymentStatus newStatus, CancellationToken ct)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));
        }

        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with id '{orderId}' not found");
        }

        order.UpdatePaymentStatus(newStatus);

        await _orderRepository.UpdateOrderAsync(order);

        var paymentStatusUpdatedEvent = new OrderPaymentStatusUpdatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            OrderId = order.OrderId,
            PaymentStatus = newStatus.ToString()
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = paymentStatusUpdatedEvent.EventId,
            Type = nameof(OrderPaymentStatusUpdatedEvent),
            RoutingKey = "orders.paymentStatusUpdated",
            Payload = JsonSerializer.Serialize(paymentStatusUpdatedEvent),
            OccurredAt = paymentStatusUpdatedEvent.OccuredAt
        }, ct);

        await _orderRepository.Save();

        return order;
    }

    public async Task<Order> UpdateDeliveryStatusAsync(Guid orderId, DeliveryStatus newStatus, CancellationToken ct)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));
        }

        var order = await _orderRepository.GetOrderByIdAsync(orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with id '{orderId}' not found");
        }

        order.UpdateDeliveryStatus(newStatus);

        await _orderRepository.UpdateOrderAsync(order);

        var deliveryStatusUpdatedEvent = new OrderDeliveryStatusUpdatedEvent
        {
            EventId = Guid.NewGuid(),
            OccuredAt = DateTimeOffset.UtcNow,
            OrderId = order.OrderId,
            DeliveryStatus = newStatus.ToString()
        };

        await _context.OutboxMessages.AddAsync(new OutboxMessage
        {
            EventId = deliveryStatusUpdatedEvent.EventId,
            Type = nameof(OrderDeliveryStatusUpdatedEvent),
            RoutingKey = "orders.deliveryStatusUpdated",
            Payload = JsonSerializer.Serialize(deliveryStatusUpdatedEvent),
            OccurredAt = deliveryStatusUpdatedEvent.OccuredAt
        }, ct);

        await _orderRepository.Save();

        return order;
    }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public OrderItemDto()
    {
        ProductName = string.Empty;
    }
}

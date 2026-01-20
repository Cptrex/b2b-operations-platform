namespace Platform.Service.Orders.Infrastructure.Logging;

public static class LoggingAction
{
    public const string CreateOrder = "order.create";
    public const string CreateCustomer = "order.createCustomer";
    public const string ConfirmOrder = "order.confirm";
    public const string CancelOrder = "order.cancel";
    public const string PaymentStatus = "payment.status";
    public const string UpdateDeliveryStatus = "order.updateDeliveryStatus";
}

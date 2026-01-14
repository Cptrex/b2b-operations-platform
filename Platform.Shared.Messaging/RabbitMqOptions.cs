namespace Platform.Service.Business.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";

    public string ExchangeName { get; set; } = "platform.events";
    public string QueueName { get; set; } = "orders-service.events";

    public ushort PrefetchCount { get; set; } = 16;
    public string[] BindingKeys { get; set; } = [];
}
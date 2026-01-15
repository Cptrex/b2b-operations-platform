namespace Platform.Service.Business.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    public string Exchange { get; set; } = "platform.events";
    public string ExchangeName { get; set; } = "platform.events";
    public string QueueName { get; set; } = "orders-service.events";

    public ushort PrefetchCount { get; set; } = 16;
    public string[] BindingKeys { get; set; } = [];
}
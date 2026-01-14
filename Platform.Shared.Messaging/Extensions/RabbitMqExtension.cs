using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Service.Business.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace Platform.Shared.Messaging.Extensions;

public static class RabbitMqExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRabbitMq(IConfiguration config)
        {
            var section = config.GetSection("RabbitMQ");

            services.Configure<RabbitMqOptions>(option =>
            {
                option.HostName = section["Host"];
                option.Port = int.TryParse(section["Port"], out var port) ? port : 5672;
                option.UserName = section["Username"];
                option.Password = section["Password"];
                option.ExchangeName = section["ExchangeName"];
                option.QueueName = section["QueueName"];
                option.PrefetchCount = ushort.TryParse(section["PrefetchCount"], out var prefetchCount) ? prefetchCount : (ushort)16;
                option.BindingKeys = section.GetSection("BindingKeys").Get<string[]>() ?? [];
            });

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = section["HostName"],
                    Port = int.TryParse(section["Port"], out var port) ? port : 5672,
                    UserName = section["UserName"],
                    Password = section["Password"],
                    VirtualHost = string.IsNullOrWhiteSpace(section["VirtualHost"]) ? "/" : section["VirtualHost"],
                };

                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            services.AddHostedService<RabbitMqConsumerHostedService>();

            return services;
        }
    }
}
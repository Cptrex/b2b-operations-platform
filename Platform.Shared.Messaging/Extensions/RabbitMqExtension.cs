using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Platform.Service.Business.Infrastructure.Messaging;
using Platform.Shared.Messaging.Contracts;
using RabbitMQ.Client;

namespace Platform.Shared.Messaging.Extensions;

public static class RabbitMqExtension
{
    extension(IServiceCollection services)
    {
        private IServiceCollection AddRabbitMqCore(IConfiguration config)
        {
            services.AddOptions<RabbitMqOptions>().Bind(config.GetSection("RabbitMQ"));

            services.TryAddSingleton<IConnection>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                var factory = new ConnectionFactory
                {
                    HostName = opt.Host,
                    Port = opt.Port,
                    UserName = opt.Username,
                    Password = opt.Password,
                    VirtualHost = opt.VirtualHost
                };

                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            return services;
        }

        public IServiceCollection AddRabbitMqPublisher(IConfiguration config)
        {
            services.AddRabbitMqCore(config);

            services.TryAddSingleton<IRabbitMqMessagePublisher, RabbitMqPublisher>();

            return services;
        }

        public IServiceCollection AddRabbitMqConsumer(IConfiguration config)
        {
            services.AddRabbitMqCore(config);

            services.AddHostedService<RabbitMqConsumerHostedService>();

            return services;
        }
    }
}
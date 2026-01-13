using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Shared.Cache.Contracts;

namespace Platform.Shared.Cache.Extensions;

public static class CachingExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRadisCacheProvider(IConfiguration config)
        {
            var section = config.GetSection(config["Radis"]);

            var host = section["Host"];
            var port = section["Port"];
            var password = section["Password"];

            var configuration = string.IsNullOrWhiteSpace(password) ? $"{host}:{port}" : $"{host}:{port},password={password}";

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration;
                options.InstanceName = "MyApp:";
            });

            services.AddSingleton<ICacheProvider, RedisCacheProvider>();

            return services;
        }
    }
}
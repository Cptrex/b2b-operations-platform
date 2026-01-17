using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Platform.Logging.MongoDb.Contracts;

namespace Platform.Logging.MongoDb.Extensions;

public static class MongoDbLoggingExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMongoDbLogging(IConfiguration configuration)
        {
            services.Configure<MongoDbOptions>(configuration.GetSection("MongoDB"));

            services.TryAddSingleton<IMongoClient>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;

                if (string.IsNullOrWhiteSpace(opt.ConnectionString))
                    throw new InvalidOperationException("MongoDB:ConnectionString is empty");

                return new MongoClient(opt.ConnectionString);
            });

            services.TryAddSingleton<IMongoDatabase>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;

                if (string.IsNullOrWhiteSpace(opt.Database))
                {
                    throw new InvalidOperationException("MongoDB:Database is empty");
                }

                var client = sp.GetRequiredService<IMongoClient>();

                return client.GetDatabase(opt.Database);
            });

            services.TryAddSingleton<IMongoCollection<LogDocument>>(sp =>
            {
                var opt = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
                var db = sp.GetRequiredService<IMongoDatabase>();

                if (string.IsNullOrWhiteSpace(opt.Collection))
                {
                    throw new InvalidOperationException("MongoDB:Collection is empty");
                }

                return db.GetCollection<LogDocument>(opt.Collection);
            });

            services.AddScoped<ILoggingService, MongoDbLoggingService>();

            return services;
        }
    }
}
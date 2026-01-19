using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Platform.Identity.Abstractions.Contracts;

namespace Platform.Identity.Http;

public static class IdentityHttpExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIdentityHttpActor()
        {
            services.AddHttpContextAccessor();
            services.TryAddScoped<IActorProvider, HttpActorProvider>();

            return services;
        }
    }
}
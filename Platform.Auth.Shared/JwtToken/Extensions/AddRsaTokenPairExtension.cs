using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Platform.Auth.Shared.JwtToken.Options;
using Platform.Auth.Shared.JwtToken.Contracts;

namespace Platform.Auth.Shared.JwtToken.Extensions;

public static class AddRsaTokenPairExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServiceTokenIssuer(IConfiguration config, string sectionName)
        {
            var section = config.GetSection(sectionName);

            services.Configure<TokenOptions>(options =>
            {
                options.Issuer = section["Issuer"];
                options.PrivateKeyPath = section["PrivateKeyPath"];
                options.PublicKeyPath = section["PublicKeyPath"];
                options.ExpiresAccessTokenMinutes = int.TryParse(section["ExpiresAccessTokenMinutes"], out var at) ? at : 0;
                options.ExpiresRefreshTokenMinutes = int.TryParse(section["ExpiresRefreshTokenMinutes"], out var rt) ? rt : 0;
            });

            services.AddSingleton<IServiceTokenIssuer, RsaServiceTokenIssuer>();

            return services;
        }

        public IServiceCollection AddClientTokenIssuer(IConfiguration config, string sectionName)
        {
            var section = config.GetSection(sectionName);

            services.Configure<TokenOptions>(options =>
            {
                options.Issuer = section["Issuer"];
                options.PrivateKeyPath = section["PrivateKeyPath"];
                options.PublicKeyPath = section["PublicKeyPath"];
                options.ExpiresAccessTokenMinutes = int.TryParse(section["ExpiresAccessTokenMinutes"], out var at) ? at : 0;
                options.ExpiresRefreshTokenMinutes = int.TryParse(section["ExpiresRefreshTokenMinutes"], out var rt) ? rt : 0;
            });

            services.AddSingleton<IClientTokenIssuer, RsaClientTokenIssuer>();

            return services;
        }
    }
}
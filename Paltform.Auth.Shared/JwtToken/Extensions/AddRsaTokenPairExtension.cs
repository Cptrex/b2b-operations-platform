using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Paltform.Auth.Shared.JwtToken.Options;
using Paltform.Auth.Shared.JwtToken.Results;

namespace Paltform.Auth.Shared.JwtToken.Extensions;

public static class AddRsaTokenPairExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRsaTokenIssuer(IConfiguration config, string sectionName)
        {
            var section = config.GetSection(sectionName);

            services.Configure<TokenOptions>(options =>
            {
                options.Issuer = section["Issuer"];
                options.PrivateKeyPath = section["PrivateKeyPath"];
                options.PublicKeyPath = section["PublicKeyPath"];
                options.ExpiresMinutes = int.TryParse(section["ExpiresMinutes"], out var m) ? m : 0;
            });

            services.AddSingleton<ITokenIssuer, RsaTokenIssuer>();

            return services;
        }
    }
}
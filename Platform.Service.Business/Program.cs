using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Platform.Service.Business.Application;
using Platform.Service.Business.Application.Security;
using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Http;
using Platform.Service.Business.Infrastructure.Http.Clients;
using Platform.Service.Business.Infrastructure.Http.Policies;
using Platform.Service.Business.Infrastructure.Messaging;
using Platform.Service.Business.Infrastructure.Security;
using Platform.Service.Business.Infrastructure.Security.Background;
using Platform.Shared.Cache.Extensions;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Extensions;
using Polly;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BusinessContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services
    .AddAuthentication()
    .AddJwtBearer("ServiceBearer", options =>
        {
            var authPublicKeyPath = builder.Configuration["ServiceJwt:PublicKeyPath"] ?? "auth_service_public.pem";

            if (!File.Exists(authPublicKeyPath))
            {
                Console.WriteLine($"Warning: Auth.Service public key not found at {authPublicKeyPath}. Will be fetched on first token request.");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };
            }
            else
            {
                var publicKeyPem = File.ReadAllText(authPublicKeyPath);
                var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["ServiceJwt:Issuer"] ?? "auth.service",

                    ValidateAudience = false,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(5),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),

                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
                };
            }
        })
    .AddJwtBearer("ClientBearer", options =>
        {
            var businessPublicKeyPath = builder.Configuration["ClientJwt:PublicKeyPath"] ?? "auth_business_public.pem";

            if (!File.Exists(businessPublicKeyPath))
            {
                Console.WriteLine($"Warning: Auth.Business public key not found at {businessPublicKeyPath}. Client authentication will fail.");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };
            }
            else
            {
                var publicKeyPem = File.ReadAllText(businessPublicKeyPath);
                var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["ClientJwt:Issuer"] ?? "auth.business",

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["ClientJwt:Audience"] ?? "platform",

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa),

                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
                };
            }
        });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Internal", policy =>
        policy.AddAuthenticationSchemes("ServiceBearer")
              .RequireAuthenticatedUser()
              .RequireClaim("type", "service"))
    .AddPolicy("Client", policy =>
        policy.AddAuthenticationSchemes("ClientBearer")
              .RequireAuthenticatedUser()
              .RequireClaim("type", "user"));

builder.Services.AddHttpClient<IAuthClient, AuthHttpClient>("AuthService", client =>
{
    var url =
        $"{builder.Configuration["AuthService:Scheme"]}://" +
        $"{builder.Configuration["AuthService:Host"]}:" +
        $"{builder.Configuration["AuthService:Port"]}";

    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddHttpClient("InternalServices").AddHttpMessageHandler<PollyDelegatingHandler>();

builder.Services.AddSingleton<IAuthServiceTokenManager, AuthServiceTokenManager>();
builder.Services.AddSingleton<IServiceTokenProvider, ServiceTokenProvider>();
builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(PollyPolicies.BuildPolicy());

builder.Services.AddTransient<PollyDelegatingHandler>();

builder.Services.AddScoped<BusinessService>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();

builder.Services.AddRadisCacheProvider(builder.Configuration);

builder.Services.AddRabbitMqConsumer(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqMessageConsumer, BusinessRabbitMqConsumer>();

builder.Services.AddHostedService<GetAuthTokenOnStartHosted>();
builder.Services.AddHostedService<OutboxPublisherBackgroundService>();

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
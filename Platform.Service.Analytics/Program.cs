using Microsoft.IdentityModel.Tokens;
using Platform.Identity.Http;
using Platform.Logging.MongoDb.Extensions;
using Platform.Shared.Messaging.Extensions;
using Platform.Shared.Abstractions.Contracts.Auth;
using Platform.Shared.Cache.Extensions;
using Platform.Service.Analytics.Infrastructure.Http.Clients;
using Platform.Service.Analytics.Infrastructure.Security;
using Prometheus;
using System.Security.Cryptography;
using Polly;
using Platform.Service.Analytics.Infrastructure.Security.Background;
using Platform.Service.Analytics.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDbLogging(builder.Configuration);

builder.Services
    .AddAuthentication()
    .AddJwtBearer("ServiceBearer", options =>
    {
        var authPublicKeyPath = builder.Configuration["ServiceJwt:PublicKeyPath"] ?? "auth_service_public.pem";

        if (!File.Exists(authPublicKeyPath))
        {
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
        policy.AddAuthenticationSchemes("ServiceBearer").RequireAuthenticatedUser().RequireClaim("type", "service"))
    .AddPolicy("Client", policy =>
        policy.AddAuthenticationSchemes("ClientBearer").RequireAuthenticatedUser().RequireClaim("type", "user"));

builder.Services.AddRabbitMqPublisher(builder.Configuration);

builder.Services.AddIdentityHttpActor();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// register auth http client, token managers and hosted services to fetch keys/tokens
builder.Services.AddRedisCacheProvider(builder.Configuration);

builder.Services.AddHttpClient<IAuthClient, AuthHttpClient>("AuthService", client =>
{
    var url =
        $"{builder.Configuration["AuthService:Scheme"]}://" +
        $"{builder.Configuration["AuthService:Host"]}:" +
        $"{builder.Configuration["AuthService:Port"]}";

    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton<IAuthServiceTokenManager, AuthServiceTokenManager>();
builder.Services.AddSingleton<IServiceTokenProvider, ServiceTokenProvider>();
builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(PollyPolicies.BuildPolicy());
builder.Services.AddTransient<PollyDelegatingHandler>();

builder.Services.AddHostedService<FetchServiceTokenHosted>();
builder.Services.AddHostedService<FetchClientTokenHosted>();

var app = builder.Build();

app.UseHttpMetrics();
app.MapMetrics();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

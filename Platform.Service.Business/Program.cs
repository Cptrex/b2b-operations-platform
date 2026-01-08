using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Platform.Service.Business.Application;
using Platform.Service.Business.Application.Security;
using Platform.Service.Business.Domain.Business;
using Platform.Service.Business.Infrastructure.Background;
using Platform.Service.Business.Infrastructure.Db;
using Platform.Service.Business.Infrastructure.Http;
using Platform.Service.Business.Infrastructure.Http.Clients;
using Platform.Service.Business.Infrastructure.Http.Policies;
using Platform.Service.Business.Infrastructure.Security;
using Polly;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRsaKeyManager, RsaKeyManager>();
var rsaKeyManager = new RsaKeyManager(builder.Configuration);

builder.Services.AddDbContext<BusinessContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStrings:Postgres")));

builder.Services
    .AddAuthentication()
    .AddJwtBearer("ServiceBearer", options =>
        {
            var authPublicKeyPath = builder.Configuration["ServiceJwt:AuthServicePublicKeyPath"] ?? "auth_service_public.pem";
            
            // Проверяем, существует ли файл с публичным ключом Auth.Service
            if (!File.Exists(authPublicKeyPath))
            {
                Console.WriteLine($"Warning: Auth.Service public key not found at {authPublicKeyPath}. Will be fetched on first token request.");
                // Создаем временный ключ для инициализации, будет обновлен при получении токена
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

                    ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 }
                };
            }
        })
    .AddJwtBearer("ClientBearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["ClientJwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = builder.Configuration["ClientJwt:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["ClientJwt:Secret"]!)
                )
            };
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

builder.Services.AddHostedService<GetAuthTokenOnStartHosted>();
builder.Services.AddSingleton<IServiceTokenProvider, ServiceTokenProvider>();
builder.Services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(PollyPolicies.BuildPolicy());
builder.Services.AddTransient<PollyDelegatingHandler>();

builder.Services.AddScoped<BusinessService>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
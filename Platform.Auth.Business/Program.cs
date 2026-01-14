using Microsoft.EntityFrameworkCore;
using Paltform.Auth.Shared.Cryptography;
using Paltform.Auth.Shared.JwtToken.Extensions;
using Platform.Auth.Business.Application;
using Platform.Auth.Business.Domain.Account;
using Platform.Auth.Business.Infrasturcture.Cache;
using Platform.Auth.Business.Infrasturcture.Db;
using Platform.Shared.Cache.Extensions;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

var privateKeyPath = builder.Configuration["ClientJwt:PrivateKeyPath"] ?? "business_private.pem";
var publicKeyPath = builder.Configuration["ClientJwt:PublicKeyPath"] ?? "business_public.pem";

if ((!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath)) && builder.Environment.IsDevelopment())
{
    RsaKeyPairGenerator.GenerateToken(privateKeyPath, publicKeyPath);
}

builder.Services.AddDbContext<AuthBusinessContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddSingleton<IPasswordService, PasswordService>();

builder.Services.AddClientTokenIssuer(builder.Configuration, "ClientJwt");
builder.Services.AddRadisCacheProvider(builder.Configuration);

builder.Services.AddHostedService<UploadCacheJwtValidationPublicKeyHosted>();

builder.Services.AddRabbitMq(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqMessageHandler, AuthBusinessRabbitMqMessageHandler>();

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
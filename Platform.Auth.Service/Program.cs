using Paltform.Auth.Shared.Cryptography;
using Paltform.Auth.Shared.JwtToken.Extensions;
using Platform.Auth.Service.Services.ServiceToken;
using Platform.Auth.Service.Services.ServiceToken.Contracts;

var builder = WebApplication.CreateBuilder(args);

var privateKeyPath = builder.Configuration["ServiceJwt:PrivateKeyPath"] ?? "service_private.pem";
var publicKeyPath = builder.Configuration["ServiceJwt:PublicKeyPath"] ?? "service_public.pem";

RsaKeyPairGenerator.GenerateToken(privateKeyPath, publicKeyPath);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IServiceCredentialStore, ServiceTokenCredentialStore>();
builder.Services.AddRsaTokenIssuer(builder.Configuration, "ServiceJwt");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
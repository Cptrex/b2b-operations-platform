using Paltform.Auth.Shared.Cryptography;
using Paltform.Auth.Shared.JwtToken.Extensions;
using Platform.Auth.Service.Services.ServiceToken;
using Platform.Auth.Service.Services.ServiceToken.Contracts;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

var privateKeyPath = builder.Configuration["ServiceJwt:PrivateKeyPath"] ?? "service_private.pem";
var publicKeyPath = builder.Configuration["ServiceJwt:PublicKeyPath"] ?? "service_public.pem";

if ((!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath)) && builder.Environment.IsDevelopment())
{
    using var rsa = RSA.Create(2048);
    File.WriteAllText(privateKeyPath, rsa.ExportRSAPrivateKeyPem());
    File.WriteAllText(publicKeyPath, rsa.ExportRSAPublicKeyPem());
    Console.WriteLine($"Generated RSA key pair: {privateKeyPath}, {publicKeyPath}");
}

builder.Services.AddAuthentication();
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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
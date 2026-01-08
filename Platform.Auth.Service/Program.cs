using Platform.Auth.Service.Application.Security;
using Platform.Auth.Service.Infrastructure;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Генерация RSA ключей для Auth.Service если не существуют
var privateKeyPath = builder.Configuration["ServiceJwt:PrivateKeyPath"] ?? "service_private.pem";
var publicKeyPath = builder.Configuration["ServiceJwt:PublicKeyPath"] ?? "service_public.pem";

if (!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath))
{
    using var rsa = RSA.Create(2048);
    var privateKey = rsa.ExportRSAPrivateKeyPem();
    var publicKey = rsa.ExportRSAPublicKeyPem();
    
    File.WriteAllText(privateKeyPath, privateKey);
    File.WriteAllText(publicKeyPath, publicKey);
    
    Console.WriteLine($"Generated RSA key pair: {privateKeyPath}, {publicKeyPath}");
}

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IServiceCredentialStore, ServiceTokenCredentialStore>();
builder.Services.AddScoped<IServiceTokenIssuer, ServiceTokenIssuer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
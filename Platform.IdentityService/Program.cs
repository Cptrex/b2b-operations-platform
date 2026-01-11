using Paltform.Auth.Shared.Cryptography;
using Paltform.Auth.Shared.JwtToken.Extensions;
using Platform.Auth.Business.Domain.Account;
using Platform.Auth.Business.Infrasturcture.Db;

var builder = WebApplication.CreateBuilder(args);

var privateKeyPath = builder.Configuration["ClientJwt:PrivateKeyPath"] ?? "business_private.pem";
var publicKeyPath = builder.Configuration["ClientJwt:PublicKeyPath"] ?? "business_public.pem";

RsaKeyPairGenerator.GenerateToken(privateKeyPath, publicKeyPath);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddRsaTokenIssuer(builder.Configuration, "ClientJwt");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
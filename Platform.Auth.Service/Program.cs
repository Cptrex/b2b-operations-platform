using Paltform.Auth.Shared.Cryptography;
using Paltform.Auth.Shared.JwtToken.Extensions;
using Platform.Auth.Service.Services.Hosted;
using Platform.Auth.Service.Services.ServiceToken;
using Platform.Auth.Service.Services.ServiceToken.Contracts;
using Platform.Identity.Http;
using Platform.Logging.MongoDb.Extensions;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var privateKeyPath = builder.Configuration["ServiceJwt:PrivateKeyPath"] ?? "service_private.pem";
var publicKeyPath = builder.Configuration["ServiceJwt:PublicKeyPath"] ?? "service_public.pem";

if ((!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath)) && builder.Environment.IsDevelopment())
{
    RsaKeyPairGenerator.GenerateToken(privateKeyPath, publicKeyPath);
}

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IServiceCredentialStore, ServiceTokenCredentialStore>();
builder.Services.AddServiceTokenIssuer(builder.Configuration, "ServiceJwt");

builder.Services.AddHostedService<UploadCacheJwtValidationPublicKeyHosted>();

builder.Services.AddIdentityHttpActor();

builder.Services.AddMongoDbLogging(builder.Configuration);

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
using Microsoft.EntityFrameworkCore;
using Platform.Service.Search.Application;
using Platform.Service.Search.Domain.Business;
using Platform.Service.Search.Domain.User;
using Platform.Service.Search.Domain.Account;
using Platform.Service.Search.Infrastructure.Db;
using Platform.Service.Search.Infrastructure.Messaging;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SearchContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddRabbitMqConsumer(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqMessageConsumer, SearchRabbitMqConsumer>();

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
app.UseAuthorization();

app.MapControllers();

app.Run();

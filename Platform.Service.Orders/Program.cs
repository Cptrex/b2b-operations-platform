using Microsoft.EntityFrameworkCore;
using Platform.Service.Orders.Application;
using Platform.Service.Orders.Domain.Order;
using Platform.Service.Orders.Infrastructure.Db;
using Platform.Service.Orders.Infrastructure.Messaging;
using Platform.Shared.Messaging.Contracts;
using Platform.Shared.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddRabbitMqConsumer(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration);
builder.Services.AddSingleton<IRabbitMqMessageConsumer, OrdersRabbitMqConsumer>();

builder.Services.AddHostedService<OutboxPublisherBackgroundService>();

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

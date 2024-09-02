using eShop.Order.Application.Interfaces;
using eShop.Order.Application.Services;
using eShop.Order.Domain.Interfaces.Messaging;
using eShop.Order.Domain.Interfaces.Repositories;
using eShop.Order.Infrastructure.Data;
using eShop.Order.Infrastructure.Data.Repositories;
using eShop.Order.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

var OrdersCollectionName = builder.Configuration["MongoDB_OrdersCollectionName"];
var ConnectionString = builder.Configuration["MongoDB_ConnectionString"];
var DatabaseName = builder.Configuration["MongoDB_DatabaseName"];

builder.Services.AddSingleton<IOrderDatabaseSettings>(sp =>
{
    List<string> missingVariables = new List<string>();
    if (string.IsNullOrEmpty(OrdersCollectionName)) missingVariables.Add("MongoDB_OrdersCollectionName");
    if (string.IsNullOrEmpty(ConnectionString)) missingVariables.Add("MongoDB_ConnectionString");
    if (string.IsNullOrEmpty(DatabaseName)) missingVariables.Add("MongoDB_DatabaseName");

    if (missingVariables.Count > 0)
    {
        throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
    }

    return new OrderDatabaseSettings(OrdersCollectionName, ConnectionString, DatabaseName);
});

builder.Services.AddSingleton<IMessageProducer, RabbitMQProducerService>(sp =>
{
    var hostName = builder.Configuration["RabbitMQ_HostName"];
    var queueName = builder.Configuration[key: "RabbitMQ_QueueName"];
    var userName = builder.Configuration["RabbitMQ_UserName"];
    var password = builder.Configuration["RabbitMQ_Password"];

    List<string> missingVariables = new List<string>();
    if (string.IsNullOrEmpty(hostName)) missingVariables.Add("RabbitMQ_HostName");
    if (string.IsNullOrEmpty(queueName)) missingVariables.Add("RabbitMQ_QueueName");
    if (string.IsNullOrEmpty(userName)) missingVariables.Add("RabbitMQ_UserName");
    if (string.IsNullOrEmpty(password)) missingVariables.Add("RabbitMQ_Password");

    if (missingVariables.Count > 0)
    {
        throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
    }

    return new RabbitMQProducerService(new RabbitMQSettings(hostName, queueName, userName, password));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

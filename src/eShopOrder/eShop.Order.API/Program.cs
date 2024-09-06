using eShop.Order.Application.Interfaces;
using eShop.Order.Application.Services;
using eShop.Order.Domain.Interfaces;
using eShop.Order.Domain.Interfaces.Messaging;
using eShop.Order.Domain.Interfaces.Repositories;
using eShop.Order.Infrastructure.Data;
using eShop.Order.Infrastructure.Data.Repositories;
using eShop.Order.Infrastructure.Messaging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<PaymentStatusProcessingService>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();

var OrdersCollectionName = builder.Configuration["MongoDBOrdersCollectionName"];
var ConnectionString = builder.Configuration["MongoDBConnectionString"];
var DatabaseName = builder.Configuration["MongoDBDatabaseName"];

builder.Services.AddSingleton<IOrderDatabaseSettings>(sp =>
{
    List<string> missingVariables = new List<string>();
    if (string.IsNullOrEmpty(OrdersCollectionName)) missingVariables.Add("MongoDBOrdersCollectionName");
    if (string.IsNullOrEmpty(ConnectionString)) missingVariables.Add("MongoDBConnectionString");
    if (string.IsNullOrEmpty(DatabaseName)) missingVariables.Add("MongoDBDatabaseName");

    if (missingVariables.Count > 0)
    {
        throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
    }

    return new OrderDatabaseSettings(OrdersCollectionName, ConnectionString, DatabaseName);
});

builder.Services.AddScoped<IMessageProducer, RabbitMQProducerService>(sp =>
{
    var hostName = builder.Configuration["RabbitMQHostName"];
    var queueName = builder.Configuration[key: "RabbitMQQueueName"];
    var userName = builder.Configuration["RabbitMQUserName"];
    var password = builder.Configuration["RabbitMQPassword"];

    List<string> missingVariables = new List<string>();
    if (string.IsNullOrEmpty(hostName)) missingVariables.Add("RabbitMQHostName");
    if (string.IsNullOrEmpty(queueName)) missingVariables.Add("RabbitMQQueueName");
    if (string.IsNullOrEmpty(userName)) missingVariables.Add("RabbitMQUserName");
    if (string.IsNullOrEmpty(password)) missingVariables.Add("RabbitMQPassword");

    if (missingVariables.Count > 0)
    {
        throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
    }

    return new RabbitMQProducerService(new RabbitMQSettings(hostName, queueName, userName, password));
});

builder.Services.AddSingleton<IMessageConsumer, RabbitMQConsumerService>(sp =>
{
    var hostName = builder.Configuration["RabbitMQHostName"];
    var userName = builder.Configuration["RabbitMQUserName"];
    var password = builder.Configuration["RabbitMQPassword"];
    var queueName = builder.Configuration["RabbitMQPaymentStatusQueueName"];

    List<string> missingVariables = new List<string>();
    if (string.IsNullOrEmpty(hostName)) missingVariables.Add("RabbitMQHostName");
    if (string.IsNullOrEmpty(queueName)) missingVariables.Add("RabbitMQPaymentStatusQueueName");
    if (string.IsNullOrEmpty(userName)) missingVariables.Add("RabbitMQUserName");
    if (string.IsNullOrEmpty(password)) missingVariables.Add("RabbitMQPassword");

    if (missingVariables.Count > 0)
    {
        throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
    }

    return new RabbitMQConsumerService(new RabbitMQSettings(hostName, queueName, userName, password));
});

//var host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices((context, services) =>
//    {
//        var hostName = context.Configuration["RabbitMQHostName"];
//        var userName = context.Configuration["RabbitMQUserName"];
//        var password = context.Configuration["RabbitMQPassword"];
//        var queueName = context.Configuration["RabbitMQPaymentStatusQueueName"];

//        List<string> missingVariables = new List<string>();
//        if (string.IsNullOrEmpty(hostName)) missingVariables.Add("RabbitMQHostName");
//        if (string.IsNullOrEmpty(queueName)) missingVariables.Add("RabbitMQPaymentStatusQueueName");
//        if (string.IsNullOrEmpty(userName)) missingVariables.Add("RabbitMQUserName");
//        if (string.IsNullOrEmpty(password)) missingVariables.Add("RabbitMQPassword");

//        if (missingVariables.Count > 0)
//        {
//            throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
//        }

//        var rabbitMQSettings = new RabbitMQSettings(hostName, queueName, userName, password);

//        services.AddSingleton(rabbitMQSettings); // RabbitMQSettings como Singleton
//        services.AddSingleton<IMessageConsumer, RabbitMQConsumerService>(); // Consumidor como Singleton
//        services.AddScoped<IOrderService, OrderService>(); // IOrderService como Scoped
//        services.AddHostedService<PaymentStatusProcessingService>();

//    })
//    .UseSerilog()
//    .Build();

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

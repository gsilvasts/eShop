using eShop.Order.Application.Interfaces;
using eShop.Order.Application.Services;
using eShop.Order.Domain.Repositories;
using eShop.Order.Infrastructure.Data;
using eShop.Order.Infrastructure.Data.Repositories;
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

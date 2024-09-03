using eShop.Payment.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<PaymentProcessingWorker>();

var host = builder.Build();
host.Run();

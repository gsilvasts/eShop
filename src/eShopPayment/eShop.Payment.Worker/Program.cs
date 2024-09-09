using eShop.Payment.Worker;
using eShop.Payment.Worker.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
{
    var hostName = hostContext.Configuration["RabbitMQHostName"];
    var queueName = hostContext.Configuration[key: "RabbitMQQueueName"];
    var paymentQueueName = hostContext.Configuration["RabbitMQPaymentStatusQueueName"];
    var userName = hostContext.Configuration["RabbitMQUserName"];
    var password = hostContext.Configuration["RabbitMQPassword"];

    List<string> missingVariables = new List<string>();
    if (string.IsNullOrEmpty(hostName)) missingVariables.Add("RabbitMQHostName");
    if (string.IsNullOrEmpty(queueName)) missingVariables.Add("RabbitMQQueueName");
    if (string.IsNullOrEmpty(paymentQueueName)) missingVariables.Add("RabbitMQPaymentStatusQueueName");
    if (string.IsNullOrEmpty(userName)) missingVariables.Add("RabbitMQUserName");
    if (string.IsNullOrEmpty(password)) missingVariables.Add("RabbitMQPassword");

    if (missingVariables.Count > 0)
    {
        throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
    }

    services.AddSingleton(new RabbitMQSettings(hostName, queueName, userName, password, paymentQueueName));
    services.AddHostedService<PaymentProcessingWorker>();

}).Build();


host.Run();

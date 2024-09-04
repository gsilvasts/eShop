using eShop.Payment.Worker;
using eShop.Payment.Worker.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<PaymentProcessingWorker>(sp =>
{
    var hostName = builder.Configuration["RabbitMQHostName"];
    var queueName = builder.Configuration[key: "RabbitMQQueueName"];
    var paymentQueueName = builder.Configuration["RabbitMQPaymentStatusQueueName"];
    var userName = builder.Configuration["RabbitMQUserName"];
    var password = builder.Configuration["RabbitMQPassword"];

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

    return new PaymentProcessingWorker(new RabbitMQSettings(hostName, queueName, userName, password, paymentQueueName));
});

var host = builder.Build();
host.Run();

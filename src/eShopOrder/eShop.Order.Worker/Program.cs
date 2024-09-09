using eShop.Order.Worker;
using eShop.Order.Worker.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var hostName = hostContext.Configuration["RabbitMQHostName"];
        var queueName = hostContext.Configuration[key: "RabbitMQQueueName"];
        var userName = hostContext.Configuration["RabbitMQUserName"];
        var password = hostContext.Configuration["RabbitMQPassword"];
        var apiBaseUrl = hostContext.Configuration["OrderApiBaseUrl"];

        List<string> missingVariables = new List<string>();
        if (string.IsNullOrEmpty(hostName)) missingVariables.Add("RabbitMQHostName");
        if (string.IsNullOrEmpty(queueName)) missingVariables.Add("RabbitMQQueueName");
        if (string.IsNullOrEmpty(userName)) missingVariables.Add("RabbitMQUserName");
        if (string.IsNullOrEmpty(password)) missingVariables.Add("RabbitMQPassword");
        if (string.IsNullOrEmpty(apiBaseUrl)) missingVariables.Add("OrderApiBaseUrl");

        if (missingVariables.Count > 0)
        {
            throw new Exception($"Missing environment variables: {string.Join(", ", missingVariables)}");
        }

        services.AddSingleton(new RabbitMQSettings(hostName, queueName, userName, password));
        services.AddHostedService<OrderStatusWorker>();

        services.AddHttpClient("OrderAPI", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

    })
    .Build();


host.Run();

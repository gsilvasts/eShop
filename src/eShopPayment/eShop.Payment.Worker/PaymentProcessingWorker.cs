using eShop.Payment.Worker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace eShop.Payment.Worker
{
    public class PaymentProcessingWorker : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private IConnection? _connection;
        private IModel? _channel;
        public PaymentProcessingWorker(RabbitMQSettings settings)
        {
            _settings = settings;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.QueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            Log.Information("Payment processing worker started");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<Orders>(message);

                if (order != null)
                {
                    Log.Information($"Order received: {message}");
                    Log.Information($"Processing payment: {order.OrderId}");

                    // Simulate payment processing
                    await Task.Delay(5000, stoppingToken);

                    var random = new Random();
                    var paymentSucceeded = random.Next(0, 2) == 1;
                    var status = paymentSucceeded ? "succeeded" : "failed";

                    Log.Information($"Payment {status}: {order.OrderId}");

                    // Publish payment status
                    var paymentStatus = new PaymentStatus
                    {
                        OrderId = order.OrderId,
                        Status = status
                    };

                    var bodyP = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paymentStatus));

                    _channel.BasicPublish(exchange: "",
                                          routingKey: _settings.PaymentStatusQueueName,
                                          basicProperties: null,
                                          body: bodyP);

                    Log.Information($"Payment status published: {order.OrderId}");

                    _channel.BasicAck(args.DeliveryTag, false);
                }
                else
                {
                    Log.Warning("Received a null or invalid payment order.");
                    _channel.BasicNack(args.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

            // Mantém o método em execução enquanto o serviço estiver ativo
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            Log.Information("Payment processing worker stopped");
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}

using eShop.Payment.Worker.Enum;
using eShop.Payment.Worker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace eShop.Payment.Worker
{
    public class PaymentProcessingWorker : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public PaymentProcessingWorker(RabbitMQSettings settings)
        {
            Console.WriteLine(JsonSerializer.Serialize(settings));
            _settings = settings;
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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Payment processing worker is running");
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

                    // Publish payment status
                    var paymentStatus = new PaymentStatusMessage
                    {
                        OrderId = order.OrderId,
                        Status = PaymentStatus.Authorized
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

        public override void Dispose()
        {
            if (_channel?.IsOpen == true)
            {
                _channel.Close();
                _channel.Dispose();
            }

            if (_connection?.IsOpen == true)
            {
                _connection.Close();
                _connection.Dispose();
            }

            base.Dispose();
        }
    }
}

using eShop.Payment.Worker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace eShop.Payment.Worker
{
    public class PaymentProcessingWorker : BackgroundService
    {
        private readonly ILogger<PaymentProcessingWorker> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _orderQueueName = "order-service-queue";
        private readonly string _paymentStatusQueueName = "payment-status-queue";

        public PaymentProcessingWorker(ILogger<PaymentProcessingWorker> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@rabbitmq:5672/") };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _orderQueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.QueueDeclare(queue: _paymentStatusQueueName,
                                 durable: true, 
                                 exclusive: false, 
                                 autoDelete: false, 
                                 arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Payment processing worker started");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<Orders>(message);

                _logger.LogInformation($"Order received: {message}");

                await ProcessPayment(order.OrderId, stoppingToken);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _orderQueueName,
                                 autoAck: false,
                                 consumer: consumer);

            await Task.CompletedTask;
        }

        private async Task ProcessPayment(string orderId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Processing payment: {orderId}");

            // Simulate payment processing
            await Task.Delay(5000, cancellationToken);

            var random = new Random();
            var paymentSucceeded = random.Next(0, 2) == 1;
            var status = paymentSucceeded ? "succeeded" : "failed";

            _logger.LogInformation($"Payment {status}: {orderId}");

            //Publish payment status
            var paymentStatus = new PaymentStatus
            {
                OrderId = orderId,
                Status = status
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paymentStatus));

            _channel.BasicPublish(exchange: "",
                                 routingKey: _paymentStatusQueueName,
                                 basicProperties: null,
                                 body: body);

            _logger.LogInformation($"Payment status published: {orderId}");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            await base.StopAsync(cancellationToken);
        }
    }
}

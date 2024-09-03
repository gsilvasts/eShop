using eShop.Order.Application.Interfaces;
using eShop.Order.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Order.Infrastructure.Messaging
{
    public class PaymentStatusConsumer : IPaymentStatusConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _paymentStatusQueueName = "payment-status-queue";
        private readonly IServiceProvider _serviceProvider;

        public PaymentStatusConsumer(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;

            _channel.QueueDeclare(queue: _paymentStatusQueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var paymentStatus = JsonSerializer.Deserialize<PaymentStatusMessage>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    orderService.UpdateOrderPaymentStatus(paymentStatus.OrderId, paymentStatus.Status);
                }
            };

            _channel.BasicConsume(queue: _paymentStatusQueueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public class PaymentStatusMessage
        {
            public string OrderId { get; set; }
            public string Status { get; set; }
        }
    }
}

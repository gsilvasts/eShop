using eShop.Order.Domain.Interfaces.Messaging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace eShop.Order.Infrastructure.Messaging
{
    public class RabbitMQProducerService : IMessageProducer
    {
        private readonly RabbitMQSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQProducerService(RabbitMQSettings settings)
        {
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

        public Task PublishAsync<T>(T message, string correlationId, CancellationToken cancellationToken)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;

            _channel.BasicPublish(exchange: "",
                                 routingKey: _settings.QueueName,
                                 basicProperties: properties,
                                 body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

using eShop.Order.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace eShop.Order.Infrastructure.Messaging
{
    public class RabbitMQConsumerService : IMessageConsumer, IDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConsumerService(RabbitMQSettings settings)
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

        public Task ConsumeAsync<T>(Func<T, Task> onMessageReceived, CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var deserializedMessage = JsonSerializer.Deserialize<T>(message);
                    if (deserializedMessage != null)
                    {
                        await onMessageReceived(deserializedMessage);
                        _channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

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

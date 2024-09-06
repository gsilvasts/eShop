using eShop.Order.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;
using System.Text;
using System.Text.Json;

namespace eShop.Order.Infrastructure.Messaging
{
    public class RabbitMQConsumerService : IMessageConsumer
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
                Password = _settings.Password,
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queue: _settings.QueueName,
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);
            }
            catch (BrokerUnreachableException ex)
            {
                Log.Error(ex, "Cannot connect to RabbitMQ: {Message}", ex.Message);
                throw; // Pode adicionar lógica de retry aqui
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during RabbitMQ setup: {Message}", ex.Message);
                throw;
            }
        }

        public async Task ConsumeAsync<T>(Func<T, Task> onMessageReceived, CancellationToken cancellationToken)
        {
            // Validar se o canal e a conexão estão abertas
            if (_channel == null || _connection == null || !_connection.IsOpen || !_channel.IsOpen)
            {
                throw new InvalidOperationException("Connection or channel is not open.");
            }

            var consumer = new AsyncEventingBasicConsumer(_channel); // Use Async para evitar bloqueios
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
                    Log.Error(ex, "Error processing message: {Message}", message);
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        public void Dispose()
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
        }
    }
}

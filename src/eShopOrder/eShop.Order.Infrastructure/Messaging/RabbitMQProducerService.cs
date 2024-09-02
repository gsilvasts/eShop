using eShop.Order.Domain.Interfaces.Messaging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eShop.Order.Infrastructure.Messaging
{
    public class RabbitMQProducerService : IMessageProducer
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQProducerService(RabbitMQSettings settings)
        {
            _settings = settings;
        }

        public async Task PublishAsync<T>(T message, string correlationId, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = _settings.HostName, UserName = _settings.UserName, Password = _settings.Password };

            await Task.Run(() =>
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _settings.QueueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    var properties = channel.CreateBasicProperties();
                    properties.CorrelationId = correlationId;

                    channel.BasicPublish(exchange: "",
                                         routingKey: _settings.QueueName,
                                         basicProperties: properties,
                                         body: body);
                }
            }, cancellationToken);
        }
    }
}

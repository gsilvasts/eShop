using eShop.Order.Worker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace eShop.Order.Worker
{
    public class OrderStatusWorker : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public OrderStatusWorker(RabbitMQSettings settings, IHttpClientFactory httpClientFactory)
        {
            Console.WriteLine(JsonSerializer.Serialize(settings));
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
                _httpClientFactory = httpClientFactory;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create RabbitMQ connection.");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Consumer started.");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deserializedMessage = JsonSerializer.Deserialize<PaymentStatusMessage>(message);

                if (deserializedMessage != null)
                {
                    Log.Information("Received message: {Message}", message);

                    var httpClient = _httpClientFactory.CreateClient("OrderAPI");

                    var content = new StringContent(JsonSerializer.Serialize(deserializedMessage.Status), Encoding.UTF8, "application/json");


                    var response = await httpClient.PostAsync($"api/orders/{deserializedMessage.OrderId}/payments", content, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        Log.Information("Payment status sent successfully for OrderId: {OrderId}", deserializedMessage.OrderId);

                        _channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        Log.Error("Failed to send payment status for OrderId: {OrderId}. StatusCode: {StatusCode}", deserializedMessage.OrderId, response.StatusCode);

                        _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                    }
                }
                else
                {
                    Log.Warning("Received null or invalid message.");
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

            Log.Information("OrderStatusWorker is consuming messages.");

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

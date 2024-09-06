using eShop.Order.Application.Interfaces;
using eShop.Order.Domain.Entities;
using eShop.Order.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace eShop.Order.Application.Services
{
    public class PaymentStatusProcessingService : BackgroundService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly IServiceProvider _serviceProvider;

        public PaymentStatusProcessingService(IMessageConsumer messageConsumer, IServiceProvider serviceProvider)
        {
            _messageConsumer = messageConsumer;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("PaymentStatusProcessingService is starting.");

            await _messageConsumer.ConsumeAsync<PaymentStatusMessage>(async message =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    await orderService.UpdateOrderPaymentStatusAsync(message.OrderId, message.Status, stoppingToken);
                }

            }, stoppingToken);

            Log.Information("PaymentStatusProcessingService is stopping.");
        }
    }
}

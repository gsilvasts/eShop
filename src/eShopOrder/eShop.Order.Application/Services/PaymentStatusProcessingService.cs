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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public PaymentStatusProcessingService(IMessageConsumer messageConsumer, IServiceScopeFactory serviceScopeFactory)
        {
            _messageConsumer = messageConsumer;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("PaymentStatusProcessingService is starting.");


            await _messageConsumer.ConsumeAsync<PaymentStatusMessage>(async message =>
            {
                Log.Information("Processing payment status message for order {OrderId}", message.OrderId);
                using (var scope = _serviceScopeFactory.CreateScope()) // Crie um escopo para serviços scoped
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    await orderService.UpdateOrderPaymentStatusAsync(message.OrderId, message.Status, stoppingToken);
                }

            }, stoppingToken);

            await Task.Delay(1000, stoppingToken);

            Log.Information("PaymentStatusProcessingService is stopping.");
        }
    }
}

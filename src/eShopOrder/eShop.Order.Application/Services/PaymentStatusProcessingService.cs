using eShop.Order.Application.Interfaces;
using eShop.Order.Domain.Entities;
using eShop.Order.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace eShop.Order.Application.Services
{
    public class PaymentStatusProcessingService : BackgroundService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly IOrderService _orderService;

        public PaymentStatusProcessingService(IMessageConsumer messageConsumer, IOrderService orderService)
        {
            _messageConsumer = messageConsumer;
            _orderService = orderService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageConsumer.ConsumeAsync<PaymentStatusMessage>(async message =>
            {
                await _orderService.UpdateOrderPaymentStatusAsync(message.OrderId, message.Status, stoppingToken);

            }, stoppingToken);

            return Task.CompletedTask;
        }
    }
}

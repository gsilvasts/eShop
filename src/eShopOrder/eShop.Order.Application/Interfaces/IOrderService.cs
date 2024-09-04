﻿using eShop.Order.Application.Services;
using eShop.Order.Domain.Enum;

namespace eShop.Order.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderViewModel> CreateOrderAsync(OrderInputModel inputModel, CancellationToken cancellationToken);
        Task DeleteOrderAsync(string orderId, CancellationToken cancellationToken);
        Task<OrderViewModel> GetOrderAsync(string orderId, CancellationToken cancellationToken);
        Task<IEnumerable<OrderViewModel>> GetOrdersAsync(CancellationToken cancellationToken);
        Task UpdateOrderAsync(string orderId, OrderInputModel order, CancellationToken cancellationToken);
        Task UpdateOrderPaymentStatusAsync(string orderId, PaymentStatus status, CancellationToken cancellationToken);
    }
}

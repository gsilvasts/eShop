using eShop.Order.Application.Interfaces;
using eShop.Order.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Order.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync(CancellationToken cancellationToken)
        {
            return await _orderService.GetOrdersAsync(cancellationToken);
        }

        [HttpGet("{orderId}")]
        public async Task<OrderViewModel> GetOrderAsync(string orderId, CancellationToken cancellationToken)
        {
            return await _orderService.GetOrderAsync(orderId, cancellationToken);
        }

        [HttpPost]
        public async Task<OrderViewModel> CreateOrderAsync(OrderInputModel inputModel, CancellationToken cancellationToken)
        {
            return await _orderService.CreateOrderAsync(inputModel, cancellationToken);
        }

        [HttpPut("{orderId}")]
        public async Task UpdateOrderAsync(string orderId, OrderInputModel order, CancellationToken cancellationToken)
        {
            await _orderService.UpdateOrderAsync(orderId, order, cancellationToken);
        }

        [HttpDelete("{orderId}")]
        public async Task DeleteOrderAsync(string orderId, CancellationToken cancellationToken)
        {
            await _orderService.DeleteOrderAsync(orderId, cancellationToken);
        }
    }
}

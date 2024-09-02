using eShop.Order.Application.Interfaces;
using eShop.Order.Domain.Entities;
using eShop.Order.Domain.Enum;
using eShop.Order.Domain.Interfaces.Repositories;

namespace eShop.Order.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderViewModel> GetOrderAsync(string orderId,  CancellationToken cancellationToken)
        {
            Orders order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

            return OrderToViewModel(order);

        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync(CancellationToken cancellationToken)
        {
            List<Orders> orders = await _orderRepository.GetAsync(cancellationToken);

            return orders.Select(OrderToViewModel);
        }

        public async Task<OrderViewModel> CreateOrderAsync(OrderInputModel inputModel, CancellationToken cancellationToken)
        {
            Orders order = InputToOrder(inputModel);

            await _orderRepository.CreateAsync(order, cancellationToken);

            return OrderToViewModel(order);
        }

        public async Task UpdateOrderAsync(string orderId, OrderInputModel order, CancellationToken cancellationToken)
        {
            Orders orderToUpdate = InputToOrder(order);

            await _orderRepository.UpdateAsync(orderId, orderToUpdate, cancellationToken);
        }

        public async Task DeleteOrderAsync(string orderId, CancellationToken cancellationToken)
        {
            await _orderRepository.RemoveAsync(orderId, cancellationToken);
        }

        private static OrderViewModel OrderToViewModel(Orders order)
        {
            return new OrderViewModel
            {
                OrderId = order.OrderId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.Items.Sum(x => x.Price * x.Quantity),
                Customer = new CustomerViewModel
                {
                    CustomerId = order.Customer.CustomerId,
                    Email = order.Customer.Email,
                    Name = order.Customer.Name,
                    Address = new AddressViewModel
                    {
                        City = order.Customer.Address.City,
                        Country = order.Customer.Address.Country,
                        Street = order.Customer.Address.Street,
                        ZipCode = order.Customer.Address.ZipCode,
                        State = order.Customer.Address.State
                    }
                },
                Items = order.Items.Select(x => new ItemViewModel
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    TotalPrice = x.Price * x.Quantity
                }).ToList(),
                Payment = new PaymentViewModel
                {
                    Method = order.Payment.Method,
                    Status = order.Payment.Status,
                    TransactionId = order.Payment.TransactionId
                }
            };
        }

        private static Orders InputToOrder(OrderInputModel inputModel)
        {
            return new Orders
            {
                OrderId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Customer = new Customer
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    Email = inputModel.Customer.Email,
                    Name = inputModel.Customer.Name,
                    Address = new Address
                    {
                        City = inputModel.Customer.Address.City,
                        Country = inputModel.Customer.Address.Country,
                        Street = inputModel.Customer.Address.Street,
                        ZipCode = inputModel.Customer.Address.ZipCode,
                        State = inputModel.Customer.Address.State
                    }
                },
                Items = inputModel.Items.Select(x => new Item
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList(),
                Payment = new Payment
                {
                    Method = inputModel.Payment.Method,
                    Status = PaymentStatus.Pending,
                    TransactionId = Guid.NewGuid().ToString()
                }
            };
        }
    }
}

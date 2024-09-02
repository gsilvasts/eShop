using eShop.Order.Domain.Entities;
using eShop.Order.Domain.Enum;

namespace eShop.Order.Application.Services
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }

        public CustomerViewModel Customer { get; set; }
        public List<ItemViewModel> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PaymentViewModel Payment { get; set; }
    }

    public class CustomerViewModel
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public AddressViewModel Address { get; set; }
    }

    public class AddressViewModel
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class ItemViewModel
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class PaymentViewModel
    {
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; }
    }
}

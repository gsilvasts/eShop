using eShop.Order.Domain.Enum;

namespace eShop.Order.Application.Services
{
    public class OrderInputModel
    {
        public CustomerInputModel Customer { get; set; }
        public List<ItemInputModel> Items { get; set; }
        public PaymentInputModel Payment { get; set; }
    }

    public class CustomerInputModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public AddressInputModel Address { get; set; }
    }

    public class AddressInputModel
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class ItemInputModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class PaymentInputModel
    {
        public PaymentMethod Method { get; set; }
    }
}

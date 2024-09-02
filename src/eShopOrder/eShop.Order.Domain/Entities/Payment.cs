using eShop.Order.Domain.Enum;

namespace eShop.Order.Domain.Entities
{
    public class Payment
    {
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; }
    }
}

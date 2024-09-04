using eShop.Order.Domain.Enum;

namespace eShop.Order.Domain.Entities
{
    public class PaymentStatusMessage
    {
        public string OrderId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}

using eShop.Order.Domain.Enum;

namespace eShop.Order.Worker.Models
{
    public class PaymentStatusMessage
    {
        public string OrderId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}

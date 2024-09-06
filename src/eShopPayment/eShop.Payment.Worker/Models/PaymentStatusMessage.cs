using eShop.Payment.Worker.Enum;

namespace eShop.Payment.Worker.Models
{
    public class PaymentStatusMessage
    {
        public string OrderId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
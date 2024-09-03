using eShop.Payment.Worker.Enum;

namespace eShop.Payment.Worker.Models
{
    public class Payment
    {
        public PaymentMethod Method { get; set; }
        public Enum.PaymentStatus Status { get; set; }
        public string TransactionId { get; set; }
    }
}

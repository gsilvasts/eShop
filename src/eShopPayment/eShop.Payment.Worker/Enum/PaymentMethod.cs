using System.ComponentModel;
using System.Text.Json.Serialization;

namespace eShop.Payment.Worker.Enum
{
    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        Cash,
        Check
    }
}

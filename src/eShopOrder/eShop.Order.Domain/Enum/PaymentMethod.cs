using System.ComponentModel;
using System.Text.Json.Serialization;

namespace eShop.Order.Domain.Enum
{
    public enum PaymentMethod
    {
        [JsonPropertyName("CreditCard")]
        CreditCard,
        [JsonPropertyName("PayPal")]
        PayPal,
        [JsonPropertyName("Cash")]
        Cash,
        [JsonPropertyName("Check")]
        Check
    }
}

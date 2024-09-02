namespace eShop.Order.Domain.Enum
{
    public enum PaymentStatus
    {
        Pending,
        Authorized,
        Paid,
        PartiallyRefunded,
        Refunded,
        Voided,
        Failed
    }
}

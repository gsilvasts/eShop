namespace eShop.Payment.Worker.Enum
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

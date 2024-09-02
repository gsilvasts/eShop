namespace eShop.Order.Domain.Interfaces.Messaging
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(T message, string correlationId, CancellationToken cancellationToken);
    }
}

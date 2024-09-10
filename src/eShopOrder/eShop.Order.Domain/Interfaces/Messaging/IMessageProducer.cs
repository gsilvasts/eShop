namespace eShop.Order.Domain.Interfaces.Messaging
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(T message, CancellationToken cancellationToken);
    }
}

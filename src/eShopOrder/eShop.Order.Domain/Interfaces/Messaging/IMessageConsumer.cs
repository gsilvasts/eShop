
namespace eShop.Order.Domain.Interfaces
{
    public interface IMessageConsumer
    {
        Task ConsumeAsync<T>(Func<T, Task> onMessageReceived, CancellationToken cancellationToken);
    }
}

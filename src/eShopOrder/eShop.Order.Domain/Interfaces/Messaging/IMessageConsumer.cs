
namespace eShop.Order.Domain.Interfaces
{
    public interface IMessageConsumer
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}

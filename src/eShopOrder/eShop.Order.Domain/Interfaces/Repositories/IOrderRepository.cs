using eShop.Order.Domain.Entities;

namespace eShop.Order.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Orders> CreateAsync(Orders order, CancellationToken cancellationToken);
        Task<List<Orders>> GetAsync(CancellationToken cancellationToken);
        Task<Orders> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task RemoveAsync(string id, CancellationToken cancellationToken);
        Task RemoveManyAsync(Orders orderIn, CancellationToken cancellationToken);
        Task UpdateAsync(string id, Orders orderIn, CancellationToken cancellationToken);
    }
}

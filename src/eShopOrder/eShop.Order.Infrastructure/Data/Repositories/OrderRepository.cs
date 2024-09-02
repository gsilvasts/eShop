using eShop.Order.Domain.Entities;
using eShop.Order.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace eShop.Order.Infrastructure.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Orders> _orders;

        public OrderRepository(IOrderDatabaseSettings settings)
        {
            _orders = settings.MongoCollection;
        }

        public async Task<List<Orders>> GetAsync(CancellationToken cancellationToken) =>
            await(await _orders.FindAsync(order => true, cancellationToken: cancellationToken)).ToListAsync();

        public async Task<Orders> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await( await _orders.FindAsync(order => order.OrderId == id, cancellationToken: cancellationToken)).FirstOrDefaultAsync();
        }
        public async Task<Orders> CreateAsync(Orders order, CancellationToken cancellationToken)
        {
            await _orders.InsertOneAsync(order, cancellationToken: cancellationToken);
            return order;
        }

        public async Task UpdateAsync(string id, Orders orderIn, CancellationToken cancellationToken) =>
            await _orders.ReplaceOneAsync(order => order.OrderId == id, orderIn, cancellationToken: cancellationToken);

        public async Task RemoveManyAsync(Orders orderIn, CancellationToken cancellationToken) =>
            await _orders.DeleteManyAsync(order => order.OrderId == orderIn.OrderId, cancellationToken);

        public async Task RemoveAsync(string id, CancellationToken cancellationToken) =>
            await _orders.DeleteOneAsync(order => order.OrderId == id, cancellationToken);
    }
}


using eShop.Order.Domain.Entities;
using MongoDB.Driver;

namespace eShop.Order.Infrastructure.Data
{
    public interface IOrderDatabaseSettings
    {
        IMongoCollection<Orders> MongoCollection { get; }
    }
}

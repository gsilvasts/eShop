using eShop.Order.Domain.Entities;
using MongoDB.Driver;

namespace eShop.Order.Infrastructure.Data
{
    public class OrderDatabaseSettings : IOrderDatabaseSettings
    {
        public string OrdersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public IMongoCollection<Orders> MongoCollection { get; set; }

        public OrderDatabaseSettings(string ordersCollectionName, string connectionString, string databaseName)
        {

            OrdersCollectionName = ordersCollectionName;
            ConnectionString = connectionString;
            DatabaseName = databaseName;

            try
            {
                MongoClientSettings setting = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));

                setting.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };

                var mongoClient = new MongoClient(setting);

                var database = mongoClient.GetDatabase(DatabaseName);

                MongoCollection = database.GetCollection<Orders>(OrdersCollectionName);
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access to db server.", ex);
            }
        }
    }
}

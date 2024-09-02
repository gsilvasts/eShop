using eShop.Order.Domain.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShop.Order.Domain.Entities
{
    public class Orders
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string OrderId { get; set; }

        public Customer Customer { get; set; }
        public List<Item> Items { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Payment Payment { get; set; }
    }
}

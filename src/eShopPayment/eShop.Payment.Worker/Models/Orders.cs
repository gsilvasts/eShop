namespace eShop.Payment.Worker.Models
{
    public class Orders
    {
        public string OrderId { get; set; }

        public Customer Customer { get; set; }
        public List<Item> Items { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Payment Payment { get; set; }
    }
}

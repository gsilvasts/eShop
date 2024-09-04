namespace eShop.Payment.Worker.Models
{
    public class RabbitMQSettings
    {
        public RabbitMQSettings(string hostName, string queueName, string userName, string password, string paymentStatusQueueName)
        {
            HostName = hostName;
            QueueName = queueName;
            UserName = userName;
            Password = password;
            PaymentStatusQueueName = paymentStatusQueueName;
        }

        public string HostName { get; set; }
        public string QueueName { get; set; }
        public string PaymentStatusQueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

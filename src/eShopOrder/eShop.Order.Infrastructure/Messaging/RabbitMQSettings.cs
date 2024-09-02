namespace eShop.Order.Infrastructure.Messaging
{
    public class RabbitMQSettings
    {
        public RabbitMQSettings(string hostName, string queueName, string userName, string password)
        {
            HostName = hostName;
            QueueName = queueName;
            UserName = userName;
            Password = password;
        }

        public string HostName { get; set; }
        public string QueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }        
    }
}

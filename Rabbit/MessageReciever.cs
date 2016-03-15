using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit
{
    public class MessageReciever
    {
        public string Recieve()
        {
            string val;
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("hello", false, false, false, null);
                var get = channel.BasicGet("hello", true);
                val = Encoding.UTF8.GetString(get.Body);
            }

            return val;
        }

        public void Listen()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("hello", false, false, false, null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    
                };
                channel.BasicConsume("hello", true, consumer);
            }
        }
    }
}
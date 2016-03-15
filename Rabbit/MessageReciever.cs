using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit
{
    public class MessageReciever
    {
        public delegate void RecievedEventHandler(object sender, RecievingEventArgs e);

        public event RecievedEventHandler Recieved;

        public void StartRecieving()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("hello", false, false, false, null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //Recieved?.Invoke(this, new RecievingEventArgs {Data = message});
                    Console.WriteLine(message);
                    //ea.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume("hello", true, consumer);
            }
        }
    }
}
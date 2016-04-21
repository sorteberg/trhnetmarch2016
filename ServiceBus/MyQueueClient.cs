using System;
using System.IO;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBus
{
    public class MyQueueClient
    {
        private QueueClient _queueClient;

        public MyQueueClient()
        {
            var connectionString = File.ReadLines("ConnectionString.txt").FirstOrDefault();
            
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            const string queueName = "servicebusqueuesample";

            if (namespaceManager == null)
            {
                System.Console.WriteLine("\nUnexpected Error: NamespaceManager is NULL");
                return;
            }

            var messageFactory = MessagingFactory.CreateFromConnectionString(connectionString);
            _queueClient = messageFactory.CreateQueueClient(queueName);

            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
            }
        }

        public void SendMessage(string message)
        {
            var sendMessage = new BrokeredMessage(message);
            _queueClient.Send(sendMessage);
        }

        public Action<string> OnMessageReceived;

        public void Listen()
        {
            _queueClient.OnMessage(message =>
            {
                OnMessageReceived(message.GetBody<string>());
            });
        }
    }
}
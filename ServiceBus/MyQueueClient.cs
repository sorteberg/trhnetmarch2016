using System;
using System.IO;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ServiceBus
{
    public class MyQueueClient
    {
        public const string TopicName = "Messages";
        public const string SubscriptionFilterKey = "Recipient";

        public void SendMessage(string recipient, string message)
        {
            var topicClient = GetTopicClient();
            var sendMessage = new BrokeredMessage(message);
            sendMessage.Properties[SubscriptionFilterKey] = recipient;
            sendMessage.Properties["guid"] = Guid.NewGuid();
            topicClient.Send(sendMessage);
        }

        private TopicClient GetTopicClient()
        {
            var connectionString = GetConnectionString();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var messageFactory = MessagingFactory.CreateFromConnectionString(connectionString);

            var topicClient = messageFactory.CreateTopicClient(TopicName);

            if (!namespaceManager.TopicExists(TopicName))
            {
                namespaceManager.CreateTopic(TopicName);
            }
            return topicClient;
        }

        private SubscriptionClient GetSubscriptionClient(string recipient)
        {
            var connectionString = GetConnectionString();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var messageFactory = MessagingFactory.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(TopicName))
            {
                namespaceManager.CreateTopic(TopicName);
            }
            var subscriptionClient = messageFactory.CreateSubscriptionClient(TopicName, recipient);

            var recipientFilter = new SqlFilter("Recipient = " + recipient);

            if (!namespaceManager.SubscriptionExists(TopicName, recipient))
            {
                namespaceManager.CreateSubscription(TopicName, recipient, recipientFilter);
            }

            return subscriptionClient;
        }

        private static string GetConnectionString()
        {
            return File.ReadLines("ConnectionString.txt").FirstOrDefault();
        }

        public Action<string> OnMessageReceived;

        public void Listen(string recipient)
        {
            var topicClient = GetSubscriptionClient(recipient);
            topicClient.OnMessage(message =>
            {
                OnMessageReceived(message.GetBody<string>());
            });
        }
    }
}
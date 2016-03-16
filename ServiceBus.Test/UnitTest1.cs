using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceBus.Test
{
    [TestClass]
    public class UnitTest1
    {
        static string _serverFqdn;
        static readonly int HttpPort = 9355;
        static readonly int TcpPort = 9354;
        static readonly string ServiceNamespace = "ServiceBusDefaultNamespace";

        [TestMethod]
        public void TestMethod1()
        {
            _serverFqdn = System.Net.Dns.GetHostEntry(string.Empty).HostName;
            ServiceBusConnectionStringBuilder connBuilder = new ServiceBusConnectionStringBuilder();
            connBuilder.ManagementPort = HttpPort;
            connBuilder.RuntimePort = TcpPort;
            connBuilder.Endpoints.Add(new UriBuilder() { Scheme = "sb", Host = _serverFqdn, Path = ServiceNamespace}.Uri);
            connBuilder.StsEndpoints.Add(new UriBuilder() { Scheme = "https", Host = _serverFqdn, Port = HttpPort, Path = ServiceNamespace }.Uri);

            MessagingFactory messageFactory = MessagingFactory.CreateFromConnectionString(connBuilder.ToString());
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connBuilder.ToString());
            const string queueName = "ServiceBusQueueSample";

            if (namespaceManager == null)
            {
                Console.WriteLine("\nUnexpected Error: NamespaceManager is NULL");
                return;
            }

            if (namespaceManager.QueueExists(queueName))
            {
                namespaceManager.DeleteQueue(queueName);
            }
            namespaceManager.CreateQueue(queueName);
            QueueClient myQueueClient = messageFactory.CreateQueueClient(queueName);
            BrokeredMessage sendMessage = new BrokeredMessage("Hello World !");
            myQueueClient.Send(sendMessage);
            BrokeredMessage receivedMessage = myQueueClient.Receive(TimeSpan.FromSeconds(5));

            if (receivedMessage != null)
            {
                Console.WriteLine("Message received: Body = {0}", receivedMessage.GetBody<string>());
                receivedMessage.Complete();
            }
            messageFactory.Close();
        }

        [TestMethod]
        public void TestUsingAzureServiceBus()
        {
            var connectionString = "Endpoint=sb://trhnet-march2016.servicebus.windows.net/;SharedAccessKeyName=manager;SharedAccessKey=Agq1HH4VHWuzh5pTEPEuUPlAre5Zad8zHymmQHGsbT4=";
            
            var messageFactory = MessagingFactory.CreateFromConnectionString(connectionString);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            const string queueName = "servicebusqueuesample";

            if (namespaceManager == null)
            {
                Console.WriteLine("\nUnexpected Error: NamespaceManager is NULL");
                return;
            }

            if (namespaceManager.QueueExists(queueName))
            {
                namespaceManager.DeleteQueue(queueName);
            }
            namespaceManager.CreateQueue(queueName);

            var myQueueClient = messageFactory.CreateQueueClient(queueName);
            var sendMessage = new BrokeredMessage("Hello World !");
            myQueueClient.Send(sendMessage);
            var receivedMessage = myQueueClient.Receive(TimeSpan.FromSeconds(5));

            if (receivedMessage != null)
            {
                Console.WriteLine("Message received: Body = {0}", receivedMessage.GetBody<string>());
                receivedMessage.Complete();
            }
            messageFactory.Close();
        }
    }
}

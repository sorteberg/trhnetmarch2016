using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceBus.Test
{
    [TestClass]
    public class UnitTest1
    {
        static string ServerFQDN;
        static int HttpPort = 9355;
        static int TcpPort = 9354;
        static string ServiceNamespace = "ServiceBusDefaultNamespace";

        [TestMethod]
        public void TestMethod1()
        {
            ServerFQDN = System.Net.Dns.GetHostEntry(string.Empty).HostName;
            ServiceBusConnectionStringBuilder connBuilder = new ServiceBusConnectionStringBuilder();
            connBuilder.ManagementPort = HttpPort;
            connBuilder.RuntimePort = TcpPort;
            connBuilder.Endpoints.Add(new UriBuilder() { Scheme = "sb", Host = ServerFQDN, Path = ServiceNamespace}.Uri);
            connBuilder.StsEndpoints.Add(new UriBuilder() { Scheme = "https", Host = ServerFQDN, Port = HttpPort, Path = ServiceNamespace }.Uri);

            MessagingFactory messageFactory = MessagingFactory.CreateFromConnectionString(connBuilder.ToString());
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(connBuilder.ToString());
            const string QueueName = "ServiceBusQueueSample";

            if (namespaceManager == null)
            {
                Console.WriteLine("\nUnexpected Error: NamespaceManager is NULL");
                return;
            }

            if (namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.DeleteQueue(QueueName);
            }
            namespaceManager.CreateQueue(QueueName);
            QueueClient myQueueClient = messageFactory.CreateQueueClient(QueueName);
            BrokeredMessage sendMessage = new BrokeredMessage("Hello World !");
            myQueueClient.Send(sendMessage);
            BrokeredMessage receivedMessage = myQueueClient.Receive(TimeSpan.FromSeconds(5));

            if (receivedMessage != null)
            {
                Console.WriteLine(string.Format("Message received: Body = {0}", receivedMessage.GetBody<string>()));
                receivedMessage.Complete();
            }
            if (messageFactory != null)
            {
                messageFactory.Close();
            }
        }
    }
}

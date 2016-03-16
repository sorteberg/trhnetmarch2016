using System;
using System.Threading;

namespace ServiceBus.Console
{
    class Program
    {
        static void Main()
        {
            var client = new MyQueueClient();
            client.OnMessageReceived += System.Console.WriteLine;
            client.Listen();

            while (true)
            {
                Thread.Sleep(100);
            }
        }   
    }
}

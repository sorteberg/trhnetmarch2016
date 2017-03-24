using System;
using System.Threading;

namespace ServiceBus.Console
{
    class Program
    {
        static void Main()
        {
            System.Console.Out.WriteLine("Skriv fødselsnummer:");
            var fnr = System.Console.ReadLine();
            var client = new MyQueueClient();
            client.OnMessageReceived += System.Console.WriteLine;
            client.Listen(fnr);

            while (true)
            {
                Thread.Sleep(100);
            }
        }   
    }
}

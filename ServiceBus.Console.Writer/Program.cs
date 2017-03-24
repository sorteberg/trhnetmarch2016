using System;
using System.Linq;

namespace ServiceBus.Console.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            var fnr = System.Console.ReadLine();

            var client = new MyQueueClient();

            while (true)
            {
                var line = System.Console.ReadLine();
                var parts = line
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToList();

                client.SendMessage(parts[0], parts[1]);
            }
        }
    }
}

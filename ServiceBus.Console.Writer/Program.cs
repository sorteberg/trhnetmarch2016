namespace ServiceBus.Console.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MyQueueClient();

            while (true)
            {
                var line = System.Console.ReadLine();
                client.SendMessage(line);
            }
        }
    }
}

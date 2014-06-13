using System;

namespace client
{
    internal class Program
    {
        private static DateTime _startTime;

        private static void Main(string[] args)
        {
            int count = 1;
            int port = 3409;
            int.TryParse(args[0], out count);
            int.TryParse(args[2], out port);

            //Random random = new Random();

            var clients = new Client[count];

            _startTime = DateTime.Now;

            for (int i = 0; i < count; i++)
            {
                clients[i] = new Client(args[1], port, i, (i%14) + 1); //random.Next(1,5));
                clients[i].EndWork += EndWorkClient;
            }
        }

        private static void EndWorkClient()
        {
            Console.WriteLine("Время работы: " + (DateTime.Now - _startTime));
        }
    }
}
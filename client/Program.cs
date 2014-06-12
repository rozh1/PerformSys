using System;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;

namespace client
{
    internal class Program
    {
        private static DateTime startTime;
        private static void Main(string[] args)
        {

            int count = 1;
            int port = 3409;
            int.TryParse(args[0], out count);
            int.TryParse(args[2], out port);

            Random random = new Random();

            Client[] clients = new Client[count];

            startTime = DateTime.Now;

            for (int i = 0; i < count; i++) {
                clients[i] = new Client(args[1], port, i, (i%14)+1); //random.Next(1,5));
                clients[i].EndWork += EndWorkClient;
            }

        }

        static void EndWorkClient()
        {
            Console.WriteLine("Время работы: " + (DateTime.Now - startTime));
        }
    }
}
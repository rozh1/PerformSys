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

            clients[0] = new Client(args[1], port, 0, 1);
            clients[1] = new Client(args[1], port, 1, 2);
            clients[2] = new Client(args[1], port, 2, 3);
            clients[3] = new Client(args[1], port, 3, 4);
            clients[4] = new Client(args[1], port, 4, 5);
            clients[5] = new Client(args[1], port, 5, 6);
            clients[6] = new Client(args[1], port, 6, 7);
            clients[7] = new Client(args[1], port, 7, 8);
            clients[8] = new Client(args[1], port, 8, 9);
            clients[9] = new Client(args[1], port, 9, 10);
            clients[10] = new Client(args[1], port, 10, 11);
            clients[11] = new Client(args[1], port, 11, 12);
            clients[12] = new Client(args[1], port, 12, 13);
            clients[13] = new Client(args[1], port, 13, 14);

            for (int i = 0; i < count; i++) {
               // clients[i] = new Client(args[1], port, i, (i%14)+1); //random.Next(1,5));
                clients[i].EndWork += EndWorkClient;
            }

        }

        static void EndWorkClient()
        {
            Console.WriteLine("Время работы: " + (startTime-DateTime.Now));
        }
    }
}
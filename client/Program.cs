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
        private static void Main(string[] args)
        {

            int count = 1;
            int port = 3409;
            int.TryParse(args[0], out count);
            int.TryParse(args[2], out port);

            Random random = new Random();

            Client[] clients = new Client[count];

            for (int i = 0; i < count; i++) clients[i] = new Client(args[1], port, i, (i%5)+1); //random.Next(1,5));
        }
    }
}
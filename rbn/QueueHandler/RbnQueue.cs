using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Balancer.Common.Packet;

namespace rbn.QueueHandler
{
    static class RbnQueue
    {
        private static List<Client> clients  = new List<Client>();

        //static public void Init()
        //{
        //    clients
        //}

        static public void AddClient(Client client)
        {
            if (clients.Contains(client))
            {
                Client tmpClient = clients[clients.IndexOf(client)];
                tmpClient.Query = client.Query;
                tmpClient.AnswerPacketData = "";
            }
            else
            {
                clients.Add(client);
            }
        }

        public static void RemoveClient(Client client)
        {
            if (clients.Contains(client))
            {
                clients.Remove(client);
            }
        }

        public static Client GetClientByTcpConnection(TcpClient tcpClient)
        {
            int count = clients.Count;
            for (int i = 0; i < count; i++)
            {
                if (clients[i].Connection == tcpClient) return clients[i];
            }
            return null;
        }

        public static void ServerAnswer(int clientId, string answer)
        {
            SendAnswer(clients[clientId-1], answer);
        }

        static void SendAnswer(Client client, string answer)
        {
            var packet = new Packet(PacketType.Answer,answer);
            byte[] bytes = packet.ToBytes();
            client.AnswerPacketData = answer;
            if (client.Connection.Connected)
                client.Connection.GetStream().Write(bytes, 0, bytes.Length);
        }
    }
}

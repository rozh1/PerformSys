using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using rbn.QueueHandler;

namespace rbn.ServersHandler
{
    class Servers
    {
        private static List<Server> servers = new List<Server>();

        private static List<Thread> serversThreads = new List<Thread>();

        static public void Init()
        {
            int serverCount = int.Parse(ConfigFile.GetConfigValue("Server_count"));
            for (int i = 1; i <= serverCount; i++)
            {
                string connString = ConfigFile.GetConfigValue("Server_" + i);
                string[] conn = connString.Split(':');
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(conn[0], int.Parse(conn[1]));
                Server server = new Server();
                server.Connection = tcpClient;
                AddServer(server);
                Thread serverThread = new Thread(ServerListenThread);
                serversThreads.Add(serverThread);
                serverThread.Start(server);
            }
        }

        static public void AddServer(Server server)
        {
            if (!servers.Contains(server))
            {
                servers.Add(server);
            }
        }

        public static void RemoveServer(Server server)
        {
            if (servers.Contains(server))
            {
                servers.Remove(server);
            }
        }

        private static int _lastReadyServerIndex;
        public static Server GetNextReadyServer()
        {
            for (int i = _lastReadyServerIndex; i < servers.Count; i++)
            {
                if (servers[i].Status)
                {
                    _lastReadyServerIndex = i + 1;
                    return servers[i];
                }
            }
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i].Status)
                {
                    _lastReadyServerIndex = i;
                    return servers[i];
                }
            }
            return null;
        }

        static void ServerListenThread(object param)
        {
            var server = (Server) param;
            TcpClient connection = server.Connection;

            while (connection.Connected)
            {
                string packetData = "";
                var buffer = new byte[1400];

                try
                {
                    int count;
                    while ((count = connection.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    {
                        packetData += Encoding.ASCII.GetString(buffer, 0, count);
                        if (packetData.IndexOf("\n\r", StringComparison.Ordinal) >= 0) break;
                    }
                    var packet = new Packet(packetData);

                    switch (packet.Type)
                    {
                        case PacketType.Status:
                            var sp = new StatusPacket(packet.Data);
                            server.Status = sp.Status;
                            RbnQueue.SendRequestToServer();
                            break;
                        case PacketType.Answer:
                            RbnQueue.ServerAnswer(int.Parse(packet.ClientId),packet.Data);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Исключение при чтении запроса: " + ex.Message);
                }
            }
        }

        static public void SendRequest(Server server, string query, int clientId)
        {
            Packet packet = new Packet(PacketType.Request, query, Settings.GlobalId,Settings.RegionId,clientId);
            byte[] packetBytes = packet.ToBytes();
            if (server.Connection.Connected)
                server.Connection.GetStream().Write(packetBytes, 0, packetBytes.Length);
        }
    }
}

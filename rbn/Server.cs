using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using rbn.QueueHandler;
using rbn.ServersHandler;

namespace rbn
{
    internal class Server
    {
        private readonly TcpListener _listener;

        private readonly TcpClient _tcpClient;

        private bool serverIsLife;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            serverIsLife = true;
            
            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (serverIsLife)
            {
                _tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                Client client = new Client();
                while (_tcpClient.Connected)
                {
                    string packetData = "";
                    var buffer = new byte[1400];

                    try
                    {
                        int count;
                        while ((count = _tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                        {
                            packetData += Encoding.ASCII.GetString(buffer, 0, count);
                            if (packetData.IndexOf("\n\r", StringComparison.Ordinal) >= 0) break;
                        }

                        if (count<=0) _tcpClient.Close();

                        if (packetData.Length > 0)
                        {
                            var packet = new Packet(packetData);

                            Logger.Write("Принят запрос: " + packet.Data);
                            
                            client.Connection = _tcpClient;
                            client.Query = packet.Data;

                            RbnQueue.AddClient(client);

                            ServersHandler.Server server = Servers.GetNextReadyServer();
                            Servers.SendRequest(server,client.Query,1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Исключение при чтении запроса: "+ ex.Message);
                    }
                }
                Logger.Write("Клиент отключен:");
                RbnQueue.RemoveClient(client);
            }
        }

        /// <summary>
        /// убийца - деструктор
        /// </summary>
        ~Server()
        {
            serverIsLife = false;
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
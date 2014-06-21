using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using rbn.QueueHandler;
using rbn.ServersHandler;

namespace rbn
{
    /// <summary>
    ///     РБН
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Слушатель новых соединений (клиентов)
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        ///     признак жизн потока
        /// </summary>
        private bool _serverIsLife;

        /// <summary>
        /// Очередь регионального балансировщика
        /// </summary>
        private readonly RbnQueue _rbnQueue;

        /// <summary>
        /// Сервера региона
        /// </summary>
        private readonly Servers _servers;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            _rbnQueue = new RbnQueue();
            _servers = new Servers();
            _servers.AnswerRecivedEvent += _rbnQueue.ServerAnswer;
            _servers.SendRequestFromQueueEvent += _rbnQueue.SendRequestToServer;

            _serverIsLife = true;


            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                var t = new Thread(ClientThread);
                t.Start(tcpClient);
            }
        }

        /// <summary>
        ///     Поток обслуживания клиента
        /// </summary>
        /// <param name="param"></param>
        private void ClientThread(object param)
        {
            var tcpClient = (TcpClient) param;
            var client = new Client();
            while (tcpClient.Connected)
            {
                string packetData = "";
                var buffer = new byte[1400];

                try
                {
                    int count;
                    while ((count = tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    {
                        packetData += Encoding.ASCII.GetString(buffer, 0, count);
                        if (packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal) >= 0) break;
                    }

                    if (count <= 0) tcpClient.Close();

                    if (packetData.Length > 0)
                    {
                        var packet = new Packet(packetData);

                        //Logger.Write("Принят запрос: " + packet.Data);

                        client.Connection = tcpClient;
                        client.RequestPacketData = packet.Data;

                        _rbnQueue.AddClient(client);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Исключение при чтении запроса: " + ex.Message);
                }
            }
            Logger.Write("Клиент отключен:");
            _rbnQueue.RemoveClient(client);
        }

        /// <summary>
        ///     убийца - деструктор
        /// </summary>
        ~Server()
        {
            _serverIsLife = false;
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
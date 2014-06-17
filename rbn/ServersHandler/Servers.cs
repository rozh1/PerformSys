using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using rbn.Config;
using rbn.QueueHandler;

namespace rbn.ServersHandler
{
    /// <summary>
    /// Сервера БД
    /// </summary>
    internal class Servers
    {
        /// <summary>
        ///     Список серверов
        /// </summary>
        private static readonly List<Server> ServersList = new List<Server>();

        /// <summary>
        ///     Список потоков серверов
        /// </summary>
        private static readonly List<Thread> ServersThreads = new List<Thread>();

        /// <summary>
        /// Признак работы потока отправки
        /// </summary>
        private static bool _sendThreadLife = true;

        /// <summary>
        /// Индекс сервера, на каторый был оправлен последний запрос
        /// </summary>
        private static int _lastReadyServerIndex;

        /// <summary>
        ///     Инициализация рабоы с серверами
        /// </summary>
        public static void Init()
        {
            foreach (Config.Data.Server serverConfig in RBNConfig.Instance.Servers)
            {
                try
                {
                    var tcpClient = new TcpClient();
                    tcpClient.Connect(serverConfig.Host, (int)serverConfig.Port);
                    var server = new Server {Connection = tcpClient};
                    AddServer(server);
                    var serverThread = new Thread(ServerListenThread);
                    ServersThreads.Add(serverThread);
                    serverThread.Start(server);
                }
                catch (Exception e)
                {
                    Logger.Write("Не удалось подключиться к серверу " + serverConfig.Host + ":" + serverConfig.Port + " " + e.Message);
                }
            }

            var t = new Thread(SendThread);
            t.Start();
        }

        /// <summary>
        ///     Поток отправки запросов серверам
        /// </summary>
        private static void SendThread()
        {
            while (_sendThreadLife)
            {
                RbnQueue.SendRequestToServer();
                Thread.Sleep(100);
            }
        }

        /// <summary>
        ///     Добавление сервера в список
        /// </summary>
        /// <param name="server"></param>
        public static void AddServer(Server server)
        {
            if (!ServersList.Contains(server))
            {
                ServersList.Add(server);
            }
        }

        /// <summary>
        ///     Удаление сервера из списка
        /// </summary>
        /// <param name="server"></param>
        public static void RemoveServer(Server server)
        {
            if (ServersList.Contains(server))
            {
                server.Connection.Close();
                ServersList.Remove(server);
            }
        }

        /// <summary>
        ///     Полчение следующего свободного сервера по круговому алгоритму
        /// </summary>
        /// <returns></returns>
        public static Server GetNextReadyServer()
        {
            for (int i = _lastReadyServerIndex; i < ServersList.Count; i++)
            {
                if (ServersList[i].Status && ServersList[i].StatusRecived)
                {
                    _lastReadyServerIndex = i + 1;
                    return ServersList[i];
                }
            }
            for (int i = 0; i < ServersList.Count; i++)
            {
                if (ServersList[i].Status && ServersList[i].StatusRecived)
                {
                    _lastReadyServerIndex = i;
                    return ServersList[i];
                }
            }
            return null;
        }

        /// <summary>
        ///     Поток обработки ответов сервера
        /// </summary>
        /// <param name="param"></param>
        private static void ServerListenThread(object param)
        {
            var server = (Server) param;
            TcpClient connection = server.Connection;
            string nextPacketData = "";
            while (connection.Connected)
            {
                string packetData = "";
                var buffer = new byte[1400];

                try
                {
                    int count;
                    while ((count = connection.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    {
                        packetData += nextPacketData + Encoding.ASCII.GetString(buffer, 0, count);
                        if (packetData.Contains(Packet.PacketEnd))
                        {
                            int index = packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal);
                            nextPacketData =
                                packetData.Substring(
                                    index +
                                    Packet.PacketEnd.Length);
                            packetData =
                                packetData.Remove(index);
                            break;
                        }
                        nextPacketData = "";
                    }
                    var packet = new Packet(packetData);

                    switch (packet.Type)
                    {
                        case PacketType.Status:
                            var sp = new StatusPacket(packet.Data);
                            server.Status = sp.Status;
                            server.StatusRecived = true;
                            //if (server.Status) RbnQueue.SendRequestToServer();
                            break;
                        case PacketType.Answer:
                            var answer = new DbAnswerPacket(packet.Data);
                            RbnQueue.ServerAnswer((int)answer.ClientId, packet.Data);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Исключение при чтении ответа: " + ex.Message);
                }
            }
        }

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        /// <param name="server"></param>
        /// <param name="query"></param>
        /// <param name="clientId"></param>
        public static void SendRequest(Server server, string query, int clientId)
        {
            var packet = new DbRequestPacket(query);
            packet.ClientId = (uint)clientId;
            byte[] packetBytes = packet.GetPacket().ToBytes();
            if (server.Connection.Connected)
            {
                server.StatusRecived = false;
                server.Connection.GetStream().Write(packetBytes, 0, packetBytes.Length);
            }
        }

        /// <summary>
        ///     убийца
        /// </summary>
        ~Servers()
        {
            _sendThreadLife = false;
            foreach (Server server in ServersList)
            {
                server.Connection.Close();
            }
        }
    }
}
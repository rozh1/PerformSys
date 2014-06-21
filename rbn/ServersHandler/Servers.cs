using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Config;
using rbn.QueueHandler;

namespace rbn.ServersHandler
{
    /// <summary>
    ///     Сервера БД
    /// </summary>
    public class Servers
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
        ///     Признак работы потока отправки
        /// </summary>
        private static bool _sendThreadLife = true;

        /// <summary>
        ///     Индекс сервера, на каторый был оправлен последний запрос
        /// </summary>
        private static int _lastReadyServerIndex;

        /// <summary>
        ///     Мьтекс отправки данных серверу
        /// </summary>
        private static readonly Mutex SendMutex = new Mutex();

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
                    tcpClient.Connect(serverConfig.Host, (int) serverConfig.Port);
                    var server = new Server {Connection = tcpClient};
                    AddServer(server);
                    var serverThread = new Thread(ServerListenThread);
                    ServersThreads.Add(serverThread);
                    serverThread.Start(server);
                }
                catch (Exception e)
                {
                    Logger.Write("Не удалось подключиться к серверу " + serverConfig.Host + ":" + serverConfig.Port +
                                 " " + e.Message);
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
                SendRequestToServer(RbnQueue.Queue);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        ///     Добавление сервера в список
        /// </summary>
        /// <param name="server"></param>
        private static void AddServer(Server server)
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
        private static void RemoveServer(Server server)
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
        private static Server GetNextReadyServer()
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
            while (connection.Connected)
            {
                Packet packet = PacketTransmitHelper.Recive(connection.GetStream());

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
                        RbnQueue.ServerAnswer((int) answer.ClientId, packet.Data);
                        break;
                }
            }
        }

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        /// <param name="server"></param>
        /// <param name="query"></param>
        /// <param name="clientId"></param>
        private static void SendRequest(Server server, string query, int clientId)
        {
            var packet = new DbRequestPacket(query) {ClientId = (uint) clientId};
            if (!server.Connection.Connected) return;
            if (PacketTransmitHelper.Send(packet.GetPacket(), server.Connection.GetStream()))
                server.StatusRecived = false;
        }

        /// <summary>
        ///     Выбор клиента и отправка его запроса на сервер
        /// </summary>
        private static void SendRequestToServer(Queue<QueueEntity> queue)
        {
            SendMutex.WaitOne(1000);
            if (queue.Count > 0)
            {
                QueueEntity qe = queue.Peek();
                if (SendRequest(qe)) queue.Dequeue();
            }
            SendMutex.ReleaseMutex();
        }

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        private static bool SendRequest(QueueEntity queueEntity)
        {
            Server server = GetNextReadyServer();
            if (server != null)
            {
                SendRequest(server, queueEntity.Query, queueEntity.ClientId);
                Client client = RbnQueue.GetClientById(queueEntity.ClientId);
                if (client == null) return false;
                client.RequestSended = true;
                Logger.Write("Отправлен запрос от клиента " + client.Id);
            }
            else return false;
            return true;
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
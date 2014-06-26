using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Interfaces;
using rbn.QueueHandler;

namespace rbn.ServersHandler
{
    /// <summary>
    ///     Сервера БД
    /// </summary>
    public class Servers : IServer, IDisposable
    {
        /// <summary>
        ///     Список серверов
        /// </summary>
        private readonly List<Server> _serversList;

        /// <summary>
        ///     Список потоков серверов
        /// </summary>
        private readonly List<Thread> _serversThreads;

        /// <summary>
        ///     Индекс сервера, на каторый был оправлен последний запрос
        /// </summary>
        private int _lastReadyServerIndex;

        /// <summary>
        ///     Признак работы потока отправки
        /// </summary>
        private bool _sendThreadLife = true;

        /// <summary>
        ///     Признак работы потока приема соединений от серверов
        /// </summary>
        private bool _serverIsLife = true;

        /// <summary>
        ///     Слушатель соедиенений
        /// </summary>
        private readonly TcpListener _listener;

        public Servers(int port)
        {
            _serversList = new List<Server>();
            _serversThreads = new List<Thread>();

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write("Начато прослушивание серверов " + IPAddress.Any + ":" + port);

            var thread = new Thread(SendThread);
            thread.Start();

            thread = new Thread(ServerThread);
            thread.Start();
        }

        /// <summary>
        ///     Событие иницирования отправки запроса из очереди
        /// </summary>
        public event Action<IServer> SendRequestFromQueueEvent;

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        public bool SendRequest(QueueEntity queueEntity)
        {
            Server server = GetNextReadyServer();
            if (server != null)
            {
                SendRequest(server, queueEntity.RequestData, queueEntity.ClientId);
                Logger.Write("Отправлен запрос от клиента " + queueEntity.ClientId);
            }
            else return false;
            return true;
        }

        /// <summary>
        ///     Событие получения ответа
        /// </summary>
        public event Action<int, DbAnswerPacket> AnswerRecivedEvent;

        /// <summary>
        ///     Поток отправки запросов серверам
        /// </summary>
        private void SendThread()
        {
            while (_sendThreadLife)
            {
                if (SendRequestFromQueueEvent != null) SendRequestFromQueueEvent(this);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        ///     Поток отправки запросов серверам
        /// </summary>
        private void ServerThread()
        {
            while (_sendThreadLife)
            {
                while (_serverIsLife)
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    Logger.Write("Подключен сервер");
                    var server = new Server { Connection = tcpClient };
                    AddServer(server);
                    var serverThread = new Thread(ServerListenThread);
                    _serversThreads.Add(serverThread);
                    serverThread.Start(server);
                }
            }
        }
        /// <summary>
        ///     Добавление сервера в список
        /// </summary>
        /// <param name="server"></param>
        private void AddServer(Server server)
        {
            if (!_serversList.Contains(server))
            {
                _serversList.Add(server);
            }
        }

        /// <summary>
        ///     Удаление сервера из списка
        /// </summary>
        /// <param name="server"></param>
        private void RemoveServer(Server server)
        {
            if (_serversList.Contains(server))
            {
                server.Connection.Close();
                _serversList.Remove(server);
            }
        }

        /// <summary>
        ///     Получение следующего свободного сервера по круговому алгоритму
        /// </summary>
        /// <returns></returns>
        private Server GetNextReadyServer()
        {
            for (int i = _lastReadyServerIndex; i < _serversList.Count; i++)
            {
                if (_serversList[i].Status && _serversList[i].StatusRecived)
                {
                    _lastReadyServerIndex = i + 1;
                    return _serversList[i];
                }
            }
            for (int i = 0; i < _serversList.Count; i++)
            {
                if (_serversList[i].Status && _serversList[i].StatusRecived)
                {
                    _lastReadyServerIndex = i;
                    return _serversList[i];
                }
            }
            return null;
        }

        /// <summary>
        ///     Поток обработки ответов сервера
        /// </summary>
        /// <param name="param"></param>
        private void ServerListenThread(object param)
        {
            var server = (Server) param;
            TcpClient connection = server.Connection;
            while (connection.Connected)
            {
                Packet packet = PacketTransmitHelper.Recive(connection.GetStream());

                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.ServerStatus:
                            var sp = new ServerStatusPacket(packet.Data);
                            server.Status = sp.Status;
                            server.StatusRecived = true;
                            break;
                        case PacketType.Answer:
                            var answer = new DbAnswerPacket(packet.Data);
                            if (AnswerRecivedEvent != null)
                                AnswerRecivedEvent((int) answer.ClientId, new DbAnswerPacket(packet.Data));
                            break;
                    }
                }
                else
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        /// <param name="server"></param>
        /// <param name="query"></param>
        /// <param name="clientId"></param>
        private void SendRequest(Server server, string query, int clientId)
        {
            var packet = new DbRequestPacket(query) {ClientId = (uint) clientId};
            if (!server.Connection.Connected) return;
            if (PacketTransmitHelper.Send(packet.GetPacket(), server.Connection.GetStream()))
                server.StatusRecived = false;
        }

        /// <summary>
        ///     убийца
        /// </summary>
        ~Servers()
        {
            Dispose();
        }

        /// <summary>
        /// Приказано уничтожить
        /// </summary>
        public void Dispose()
        {
            _serverIsLife = false;
            _sendThreadLife = false;
            foreach (Server server in _serversList)
            {
                RemoveServer(server);
            }
        }
    }
}
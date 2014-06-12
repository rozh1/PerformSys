using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using rbn.ServersHandler;

namespace rbn.QueueHandler
{
    static class RbnQueue
    {
        /// <summary>
        /// Список клиентов (очередь выполнения)
        /// </summary>
        private static readonly List<Client> Clients  = new List<Client>();

        static Queue<QueueEntity> _queue = new Queue<QueueEntity>(); 

        private static int _id = 1;

        static object clientSyncObject = new object();

        /// <summary>
        /// Добавление клиента в конец
        /// </summary>
        /// <param name="client">клиент</param>
        static public void AddClient(Client client)
        {
            lock (clientSyncObject)
            {
                if (Clients.Contains(client))
                {
                    Client tmpClient = Clients[Clients.IndexOf(client)];
                    tmpClient.Query = client.Query;
                    tmpClient.AnswerPacketData = "";
                    _queue.Enqueue(new QueueEntity() {ClientId = tmpClient.Id, Query = tmpClient.Query});
                }
                else
                {
                    client.Id = _id++;
                    Clients.Add(client);
                    _queue.Enqueue(new QueueEntity() {ClientId = client.Id, Query = client.Query});
                }
            }
        }

        /// <summary>
        /// Удаление клиента
        /// </summary>
        /// <param name="client">клиент</param>
        public static void RemoveClient(Client client)
        {
            lock (clientSyncObject)
            {
                if (Clients.Contains(client))
                {
                    Clients.Remove(client);
                }
            }
        }

        /// <summary>
        /// Ответ сервера
        /// </summary>
        /// <param name="clientId">номер клиента</param>
        /// <param name="answer">пакет ответа</param>
        public static void ServerAnswer(int clientId, string answer)
        {
            Logger.Write("Получен ответ для клиента " + clientId);
            try
            {
                lock (clientSyncObject)
                {
                    int count = Clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (Clients[i].Id == clientId)
                        {
                            SendAnswer(Clients[i], answer);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Исключение при передаче ответа клиенту" + ex.Message);
            }
        }

        /// <summary>
        /// отправка ответа клиенту
        /// </summary>
        /// <param name="client">клиент</param>
        /// <param name="answer">пакет ответа</param>
        static void SendAnswer(Client client, string answer)
        {
            var packet = new Packet(PacketType.Answer,answer);
            byte[] bytes = packet.ToBytes();
            client.AnswerPacketData = answer;
            if (client.Connection.Connected)
                client.Connection.GetStream().Write(bytes, 0, bytes.Length);
        }

        private static Mutex sendMutex = new Mutex();

        /// <summary>
        /// Выбор клиента и отправка его запроса на сервер
        /// </summary>
        static public void SendRequestToServer()
        {
            sendMutex.WaitOne(1000);
            if (_queue.Count > 0)
            {
                QueueEntity qe = _queue.Peek();
                if (SendRequest(qe)) _queue.Dequeue();
            }
            sendMutex.ReleaseMutex();
        }

        /// <summary>
        /// Отправка запроса серверу
        /// </summary>
        static bool SendRequest(QueueEntity queueEntity)
        {
            ServersHandler.Server server = Servers.GetNextReadyServer();
            if (server != null)
            {
                Servers.SendRequest(server, queueEntity.Query, queueEntity.ClientId);
                Client client = GetClientById(queueEntity.ClientId);
                if (client == null) return false;
                client.QuerySended = true;
                Logger.Write("Отправлен запрос от клиента " + client.Id);
            }
            else return false;
            return true;
        }

        static Client GetClientById(int Id)
        {
            lock (clientSyncObject)
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    if (Clients[i].Id == Id)
                    {
                        return Clients[i];
                    }
                }
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using rbn.ServersHandler;

namespace rbn.QueueHandler
{
    /// <summary>
    /// Очередь регионального балансировщика
    /// </summary>
    internal static class RbnQueue
    {
        /// <summary>
        ///     Список клиентов (очередь выполнения)
        /// </summary>
        private static readonly List<Client> Clients = new List<Client>();

        /// <summary>
        ///     Очередь
        /// </summary>
        public static readonly Queue<QueueEntity> Queue = new Queue<QueueEntity>();

        /// <summary>
        ///     Уникальный идентификатор пользователя
        /// </summary>
        private static int _id = 1;

        /// <summary>
        ///     Объект синхронизации работы с клиентами
        /// </summary>
        private static readonly object ClientSyncObject = new object();
        
        /// <summary>
        ///     Добавление клиента в конец
        /// </summary>
        /// <param name="client">клиент</param>
        public static void AddClient(Client client)
        {
            lock (ClientSyncObject)
            {
                if (Clients.Contains(client))
                {
                    Client tmpClient = Clients[Clients.IndexOf(client)];
                    tmpClient.RequestPacketData = client.RequestPacketData;
                    tmpClient.AnswerPacketData = "";
                    Queue.Enqueue(new QueueEntity {ClientId = tmpClient.Id, Query = tmpClient.RequestPacketData});
                }
                else
                {
                    client.Id = _id++;
                    Clients.Add(client);
                    Queue.Enqueue(new QueueEntity {ClientId = client.Id, Query = client.RequestPacketData});
                }
            }
        }

        /// <summary>
        ///     Удаление клиента
        /// </summary>
        /// <param name="client">клиент</param>
        public static void RemoveClient(Client client)
        {
            lock (ClientSyncObject)
            {
                if (Clients.Contains(client))
                {
                    Clients.Remove(client);
                }
            }
        }

        /// <summary>
        ///     Ответ сервера
        /// </summary>
        /// <param name="clientId">номер клиента</param>
        /// <param name="answer">пакет ответа</param>
        public static void ServerAnswer(int clientId, string answer)
        {
            Logger.Write("Получен ответ для клиента " + clientId);
            try
            {
                lock (ClientSyncObject)
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
        ///     отправка ответа клиенту
        /// </summary>
        /// <param name="client">клиент</param>
        /// <param name="answer">пакет ответа</param>
        private static void SendAnswer(Client client, string answer)
        {
            var packet = new Packet(PacketType.Answer, answer);
            byte[] bytes = packet.ToBytes();
            client.AnswerPacketData = answer;
            if (client.Connection.Connected)
                client.Connection.GetStream().Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///     Получение клиента по ID
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>клиент</returns>
        public static Client GetClientById(int id)
        {
            lock (ClientSyncObject)
            {
                foreach (Client client in Clients)
                {
                    if (client.Id == id)
                    {
                        return client;
                    }
                }
            }
            return null;
        }
    }
}
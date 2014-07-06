using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Interfaces;
using rbn.QueueHandler.Data;

namespace rbn.QueueHandler
{
    /// <summary>
    ///     Очередь регионального балансировщика
    /// </summary>
    public class RbnQueue
    {
        /// <summary>
        ///     Объект синхронизации работы с клиентами
        /// </summary>
        private readonly object _clientSyncObject;

        /// <summary>
        ///     Список клиентов (очередь выполнения)
        /// </summary>
        private readonly List<Client> _clients;

        /// <summary>
        ///     Очередь
        /// </summary>
        private readonly Queue<QueueEntity> _queue;

        /// <summary>
        ///     Мьтекс отправки данных серверу
        /// </summary>
        private readonly Mutex _sendMutex;

        /// <summary>
        ///     Уникальный идентификатор пользователя
        /// </summary>
        private int _id = 1;

        private List<TableSizes> _tableSizes;

        public RbnQueue()
        {
            _sendMutex = new Mutex();
            _clientSyncObject = new object();
            _queue = new Queue<QueueEntity>();
            _clients = new List<Client>();
            _tableSizes = new List<TableSizes>();
        }

        /// <summary>
        ///     Добавление клиента в конец
        /// </summary>
        /// <param name="client">клиент</param>
        public void AddClient(Client client)
        {
            lock (_clientSyncObject)
            {
                if (_clients.Contains(client))
                {
                    Client tmpClient = _clients[_clients.IndexOf(client)];
                    tmpClient.RequestPacketData = client.RequestPacketData;
                    tmpClient.AnswerPacketData = "";
                    var requestPacket = new DbRequestPacket(tmpClient.RequestPacketData)
                    {
                        ClientId = (uint) tmpClient.Id
                    };
                    _queue.Enqueue(new QueueEntity {RequestPacket = requestPacket });
                }
                else
                {
                    client.Id = _id++;
                    _clients.Add(client);
                    var requestPacket = new DbRequestPacket(client.RequestPacketData)
                    {
                        ClientId = (uint) client.Id
                    };
                    _queue.Enqueue(new QueueEntity {RequestPacket = requestPacket });
                }
            }
        }

        /// <summary>
        ///     Удаление клиента
        /// </summary>
        /// <param name="client">клиент</param>
        public void RemoveClient(Client client)
        {
            lock (_clientSyncObject)
            {
                if (_clients.Contains(client))
                {
                    _clients.Remove(client);
                }
            }
        }

        /// <summary>
        ///     Ответ сервера
        /// </summary>
        /// <param name="clientId">номер клиента</param>
        /// <param name="dbAnswerPacket">пакет ответа</param>
        public void ServerAnswer(int clientId, DbAnswerPacket dbAnswerPacket)
        {
            Logger.Write("Получен ответ для клиента " + clientId);
            try
            {
                lock (_clientSyncObject)
                {
                    int count = _clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (_clients[i].Id == clientId)
                        {
                            PacketTransmitHelper.Send(dbAnswerPacket.GetPacket(),
                                _clients[i].Connection.GetStream());
                            Client client = GetClientById(clientId);
                            if (client.DisposeAfterTransmitAnswer)
                                RemoveClient(client);
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
        ///     Получение клиента по ID
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <returns>клиент</returns>
        public Client GetClientById(int id)
        {
            lock (_clientSyncObject)
            {
                foreach (Client client in _clients)
                {
                    if (client.Id == id)
                    {
                        return client;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Выбор клиента и отправка его запроса на сервер
        /// </summary>
        public void SendRequestToServer(IServer server)
        {
            _sendMutex.WaitOne(1000);
            if (_queue.Count > 0)
            {
                QueueEntity queueEntity = _queue.Peek();
                if (server.SendRequest(queueEntity))
                {
                    Client client = GetClientById(queueEntity.ClientId);
                    if (client != null) client.RequestSended = true;
                    _queue.Dequeue();
                }
            }
            _sendMutex.ReleaseMutex();
        }

        /// <summary>
        /// Добавление информации о БД
        /// </summary>
        /// <param name="packet"></param>
        public void AddDataBaseInfo(DataBaseInfoPacket packet)
        {
            List<TableSizes> tableSizeToRemove = _tableSizes.Where(
                tableSizes => 
                    tableSizes.RegionId == packet.RegionId && tableSizes.GlobalId == packet.GlobalId
                    ).ToList();
            foreach (TableSizes tableSizes in tableSizeToRemove)
            {
                _tableSizes.Remove(tableSizes);
            }

            _tableSizes.Add(new TableSizes()
            {
                RegionId = (int)packet.RegionId, 
                GlobalId = (int)packet.GlobalId, 
                Sizes = packet.TableSizes
            });
        }
    }
}
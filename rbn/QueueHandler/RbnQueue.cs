using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Config.Data;
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

        /// <summary>
        /// Размеры таблиц БД регионов
        /// </summary>
        private readonly List<TableSizes> _tableSizes;

        private readonly PacketTransmitHelper _transmitHelper;

        public RbnQueue()
        {
            _transmitHelper = new PacketTransmitHelper();
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
                DbRequestPacket requestPacket;
                if (_clients.Contains(client))
                {
                    Client tmpClient = _clients[_clients.IndexOf(client)];
                    tmpClient.RequestPacketData = client.RequestPacketData;
                    tmpClient.AnswerPacketData = "";
                    requestPacket = new DbRequestPacket(tmpClient.RequestPacketData)
                    {
                        ClientId = (uint) tmpClient.Id
                    };
                }
                else
                {
                    client.Id = _id++;
                    _clients.Add(client);
                    requestPacket = new DbRequestPacket(client.RequestPacketData)
                    {
                        ClientId = (uint) client.Id
                    };
                }

                _queue.Enqueue(
                    new QueueEntity
                    {
                        RequestPacket = requestPacket,
                        RelationVolume = CalculateRelationsVolume(requestPacket)
                    });

                client.LogStats = new LogStats
                {
                    GlobalId = requestPacket.GlobalId,
                    RegionId = requestPacket.RegionId,
                    QueryNumber = requestPacket.QueryNumber,
                    QueueLength = _queue.Count,
                };
                client.AddedTime = DateTime.UtcNow;
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
                            _transmitHelper.Send(dbAnswerPacket.GetPacket(),
                                _clients[i].Connection.GetStream());
                            Client client = GetClientById(clientId);

                            client.LogStats.QueryExecutionTime = DateTime.UtcNow - client.SendedTime;
                            Logger.WriteCsv(client.LogStats);

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
        private Client GetClientById(int id)
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
                    if (client != null)
                    {
                        client.RequestSended = true;
                        client.SendedTime = DateTime.UtcNow;
                        client.LogStats.QueueWaitTime = DateTime.UtcNow - client.AddedTime;
                    }
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

            double dataBaseSize = packet.TableSizes.Aggregate<KeyValuePair<string, ulong>, double>(0, (current, pair) => current + pair.Value);

            _tableSizes.Add(new TableSizes
            {
                RegionId = (int)packet.RegionId, 
                GlobalId = (int)packet.GlobalId, 
                Sizes = packet.TableSizes,
                DataBaseSize = dataBaseSize/1024.0/1024.0
            });

            foreach (QueueEntity queueEntity in _queue)
            {
                queueEntity.RelationVolume = CalculateRelationsVolume(queueEntity.RequestPacket);
            }
        }

        /// <summary>
        /// Подсчитывает объем отношений в МБ
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private double CalculateRelationsVolume(DbRequestPacket packet)
        {
            TableSizes tableSizes = null;
            UInt64 relationsVolume = 0;
            foreach (TableSizes tableSize in _tableSizes)
            {
                if (tableSize.RegionId == packet.RegionId && tableSize.GlobalId == packet.GlobalId)
                    tableSizes = tableSize;
            }

            if (tableSizes != null)
            {
                string query = packet.Query.ToLower();
                var relationsList = new List<string>();
                string[] words = query.Split(new[] {' ', ',', '\t', '\n', '\r', ';'}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    if (tableSizes.Sizes.ContainsKey(word))
                        relationsList.Add(word);
                }
                foreach (KeyValuePair<string, UInt64> pair in tableSizes.Sizes)
                {
                    if (relationsList.Contains(pair.Key))
                        relationsVolume += pair.Value;
                }

            }
            return relationsVolume/1024.0/1024.0;
        }

        public double ComputeQueueWeight()
        {
            TableSizes tableSizes = null;
            foreach (TableSizes tableSize in _tableSizes)
            {
                if (tableSize.RegionId == Config.RBNConfig.Instance.RBN.RegionId && tableSize.GlobalId == Config.RBNConfig.Instance.RBN.GlobalId)
                    tableSizes = tableSize;
            }
            lock (_queue)
            {
                double requestVolume = _queue.Sum(queueEntity => queueEntity.RelationVolume);
                double normalize = Config.RBNConfig.Instance.RBN.ServersCount/
                                   (double) Config.RBNConfig.Instance.RBN.MaxServersCount;
                if (tableSizes != null) return (requestVolume/(_queue.Count*tableSizes.DataBaseSize))*normalize;
            }
            return 0;
        }
    }
}
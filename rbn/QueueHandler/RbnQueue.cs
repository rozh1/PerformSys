﻿#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
 #endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Packet.Packets;
using PerformSys.Common.Utils;
using rbn.Config;
using rbn.Config.Data.LogData;
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
        ///     Размеры таблиц БД регионов
        /// </summary>
        private readonly List<TableSizes> _tableSizes;

        private readonly PacketTransmitHelper _transmitHelper;
        
        public RbnQueue()
        {
            _transmitHelper = new PacketTransmitHelper(RBNConfig.Instance.Log.LogFile);
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
                    requestPacket = new DbRequestPacket(tmpClient.RequestPacketData);
                }
                else
                {
                    requestPacket = new DbRequestPacket(client.RequestPacketData);
                    client.Id = ComputeClientId(requestPacket);
                    _clients.Add(client);
                }

                _queue.Enqueue(
                    new QueueEntity
                    {
                        ClientId = client.Id,
                        RequestPacket = requestPacket,
                        RelationVolume = CalculateRelationsVolume(requestPacket)
                    });

                client.LogStats = new Stats(requestPacket.GlobalId, requestPacket.RegionId,
                    (int) requestPacket.ClientId, requestPacket.QueryNumber, _queue.Count);

                Logger.Write(RBNConfig.Instance.Log.QueueStatsFile,
                    new QueueStats(requestPacket.GlobalId, requestPacket.RegionId, (int) requestPacket.ClientId, _queue.Count), 
                    LogLevel.INFO);

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
        /// <param name="dbAnswerPacket">пакет ответа</param>
        public void ServerAnswer(DbAnswerPacket dbAnswerPacket)
        {
            int id = ComputeClientId(dbAnswerPacket);
            Logger.Write(RBNConfig.Instance.Log.LogFile,
                new StringLogData("Получен ответ для клиента " + id),
                LogLevel.INFO);
            try
            {
                lock (_clientSyncObject)
                {
                    int count = _clients.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (_clients[i].Id == id)
                        {
                            Client client = _clients[i];

                            _transmitHelper.Send(dbAnswerPacket.GetPacket(),
                                client.Connection.GetStream());

                            client.LogStats.QueryExecutionTime = DateTime.UtcNow - client.SendedTime;
                            Logger.Write(RBNConfig.Instance.Log.StatsFile,
                                client.LogStats,
                                LogLevel.INFO);

                            if (client.DisposeAfterTransmitAnswer)
                                RemoveClient(client);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(RBNConfig.Instance.Log.LogFile,
                    new StringLogData("Исключение при передаче ответа клиенту" + ex.Message),
                    LogLevel.ERROR);
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

                    Logger.Write(RBNConfig.Instance.Log.QueueStatsFile,
                    new QueueStats(queueEntity.RequestPacket.GlobalId, queueEntity.RequestPacket.RegionId, (int)queueEntity.RequestPacket.ClientId, _queue.Count),
                    LogLevel.INFO);
                }
            }
            _sendMutex.ReleaseMutex();
        }

        /// <summary>
        ///     Выбор клиента и отправка его запроса в МРБН
        /// </summary>
        public void SendRequestToAnotherRegion(IServer server)
        {
            _sendMutex.WaitOne(1000);
            if (_queue.Count > 0)
            {
                QueueEntity queueEntity = _queue.Peek();
                Client client = GetClientById(queueEntity.ClientId);

                if (queueEntity.RequestPacket.RegionId != RBNConfig.Instance.RBN.RegionId)
                {
                    _clients.Remove(client);
                }

                if (server.SendRequest(queueEntity))
                {
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
        ///     Добавление информации о БД
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

            if (packet.TableSizes == null)
            {
                Logger.Write(
                    RBNConfig.Instance.Log.LogFile,
                    new StringLogData(string.Format("Нет данных о объеме БД региона {0}", packet.RegionId)),
                    LogLevel.ERROR
                    );
            }
            else
            {
                double dataBaseSize = packet.TableSizes.Aggregate<KeyValuePair<string, ulong>, double>(0,
                    (current, pair) => current + pair.Value);

                _tableSizes.Add(new TableSizes
                {
                    RegionId = (int) packet.RegionId,
                    GlobalId = (int) packet.GlobalId,
                    Sizes = packet.TableSizes,
                    DataBaseSize = dataBaseSize/1024.0/1024.0
                });

                foreach (QueueEntity queueEntity in _queue)
                {
                    queueEntity.RelationVolume = CalculateRelationsVolume(queueEntity.RequestPacket);
                }
            }

        }

        /// <summary>
        ///     Подсчитывает объем отношений в МБ
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
                {
                    tableSizes = tableSize;
                    break;
                }
            }

            if (tableSizes != null)
            {
                string query = packet.Query.ToLower();
                var relationsList = new List<string>();
                string[] words = query.Split(new[] {' ', ',', '\t', '\n', '\r', ';'},
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    if (tableSizes.Sizes.ContainsKey(word))
                        relationsList.Add(word);
                }
                foreach (var pair in tableSizes.Sizes)
                {
                    if (relationsList.Contains(pair.Key))
                        relationsVolume += pair.Value;
                }
            }
            return relationsVolume/1024.0/1024.0;
        }

        public double ComputeQueueWeight()
        {
            double weight = 0;
            double normalize = RBNConfig.Instance.RBN.ServersCount / (double)RBNConfig.Instance.RBN.MaxServersCount;

            if (_queue.Count > 0)
            {
                lock (_queue)
                {
                    foreach (TableSizes tableSize in _tableSizes)
                    {
                        if (tableSize != null)
                        {
                            QueueEntity[] queueEntities = _queue.Where(queueEntity =>
                                queueEntity.RequestPacket.RegionId == tableSize.RegionId &&
                                queueEntity.RequestPacket.GlobalId == tableSize.GlobalId
                                ).ToArray();
                            if (queueEntities.Length > 0)
                            {
                                double requestVolume = queueEntities.Sum(queueEntity => queueEntity.RelationVolume);
                                weight += requestVolume / (_queue.Count * tableSize.DataBaseSize);
                            }
                        }
                    }
                }
            }
            Debug.WriteLine(weight * normalize);
            return weight*normalize;
        }

        public int ComputeClientId(PacketBase packet)
        {
            return (int) (packet.GlobalId*100000000 + packet.RegionId*1000000 + packet.ClientId);
        }
    }
}
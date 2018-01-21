#region Copyright
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

﻿using System.Collections.Generic;
using System.Threading;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Packet.Packets;
using PerformSys.Common.Utils;
using router.Config;
using router.Config.Data.LogData;
using router.Interfaces;

namespace router
{
    /// <summary>
    ///     Очередь регионального балансировщика
    /// </summary>
    public class RouterQueue
    {
        /// <summary>
        ///     Очередь
        /// </summary>
        private readonly Queue<DbRequestPacket> _queue;

        /// <summary>
        ///     Мьтекс отправки данных серверу
        /// </summary>
        private readonly Mutex _sendMutex;
        
        private readonly PacketTransmitHelper _transmitHelper;
        
        public RouterQueue()
        {
            _transmitHelper = new PacketTransmitHelper(RouterConfig.Instance.Log.LogFile);
            _sendMutex = new Mutex();
            _queue = new Queue<DbRequestPacket>();
        }

        public void AddQuery(DbRequestPacket entity)
        {
            _sendMutex.WaitOne(1000);
            _queue.Enqueue(entity);
            _sendMutex.ReleaseMutex();
        }
        
        /// <summary>
        ///     Выбор клиента и отправка его запроса на сервер
        /// </summary>
        public void SendRequestToServer(IServer server)
        {
            _sendMutex.WaitOne(1000);
            if (_queue.Count > 0)
            {
                var queueEntity = _queue.Peek();
                if (server.SendRequest(queueEntity))
                {
                    _queue.Dequeue();

                    Logger.Write(RouterConfig.Instance.Log.QueueStatsFile,
                    new QueueStats(queueEntity.GlobalId, queueEntity.RegionId, (int)queueEntity.ClientId, _queue.Count),
                    LogLevel.INFO);
                }
            }
            _sendMutex.ReleaseMutex();
        }

        public int GetQueueLength()
        {
            int count = 0;
            _sendMutex.WaitOne(1000);
            count = _queue.Count;
            _sendMutex.ReleaseMutex();
            return count;
        }
    }
}
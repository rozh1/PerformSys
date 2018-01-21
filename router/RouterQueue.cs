using System.Collections.Generic;
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
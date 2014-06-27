using System;
using Balancer.Common.Packet.Packets;
using rbn.QueueHandler;
using rbn.QueueHandler.Data;
using rbn.ServersHandler;

namespace rbn.Interfaces
{
    public interface IServer
    {
        /// <summary>
        /// Событие иницирования отправки запроса из очереди
        /// </summary>
        event Action<IServer> SendRequestFromQueueEvent;

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        bool SendRequest(QueueEntity queueEntity);
    }
}
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
        /// ������� ������������ �������� ������� �� �������
        /// </summary>
        event Action<IServer> SendRequestFromQueueEvent;

        /// <summary>
        ///     �������� ������� �������
        /// </summary>
        bool SendRequest(QueueEntity queueEntity);
    }
}
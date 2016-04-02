using System;
using Balancer.Common.Packet.Packets;

namespace router.Interfaces
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
        bool SendRequest(DbRequestPacket queueEntity);
    }
}
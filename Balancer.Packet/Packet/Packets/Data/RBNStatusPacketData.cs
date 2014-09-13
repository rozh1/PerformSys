using ProtoBuf;

namespace Balancer.Common.Packet.Packets.Data
{
    [ProtoContract]
    public class RBNStatusPacketData : PacketBase
    {
        [ProtoMember(1)]
        public double Weight { get; set; }
    }
}
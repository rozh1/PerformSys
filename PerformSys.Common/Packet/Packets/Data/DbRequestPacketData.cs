using ProtoBuf;

namespace PerformSys.Common.Packet.Packets.Data
{
    [ProtoContract]
    public class DbRequestPacketData : PacketBase
    {
        [ProtoMember(1)]
        public string Query { get; set; }

        [ProtoMember(2)]
        public int QueryNumber { get; set; }
    }
}
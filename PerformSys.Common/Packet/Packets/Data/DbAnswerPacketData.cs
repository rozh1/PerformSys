using ProtoBuf;

namespace PerformSys.Common.Packet.Packets.Data
{
    [ProtoContract]
    public class DbAnswerPacketData : PacketBase
    {
        [ProtoMember(1)]
        public byte[] DataTable { get; set; }

        [ProtoMember(2)]
        public int QueryNumber { get; set; }
    }
}
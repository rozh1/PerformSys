using Balancer.Common.Packet.Packets.Data;
using Balancer.Common.Utils;
using ProtoBuf;

namespace Balancer.Common.Packet.Packets
{
    [ProtoContract]
    [ProtoInclude(100, typeof(DataBaseInfoPacketData))]
    [ProtoInclude(200, typeof(DbAnswerPacketData))]
    [ProtoInclude(300, typeof(DbRequestPacketData))]
    public class PacketBase
    {
        [ProtoMember(1)]
        public uint GlobalId { get; set; }
        [ProtoMember(2)]
        public uint RegionId { get; set; }
        [ProtoMember(3)]
        public uint ClientId { get; set; }

        public void Deserialize(string serializedPacket)
        {
            var packetData = SerializeMapper.Deserialize<PacketBase>(serializedPacket);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }
    }
}

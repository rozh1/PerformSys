using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    [DataContract]
    public class PacketBase
    {
        [DataMember]
        public uint GlobalId { get; set; }
        [DataMember]
        public uint RegionId { get; set; }
        [DataMember]
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

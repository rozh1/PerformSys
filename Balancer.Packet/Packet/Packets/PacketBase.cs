using System.Runtime.Serialization;

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
    }
}

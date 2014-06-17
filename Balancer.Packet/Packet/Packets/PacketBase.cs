using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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

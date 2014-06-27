using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class TransmitRequestPacket : PacketBase, IPacket
    {
        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new PacketData()
            {
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.TransmitRequest, SerializePacketData());
        }

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public double Weight { get; set; }
        }
    }
}

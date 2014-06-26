using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class RBNStatusPacket : PacketBase, IPacket
    {
        public RBNStatusPacket(double weight)
        {
            Weight = weight;
        }

        public RBNStatusPacket(string serializedPacketData)
        {
            var packetData = (PacketData)SerializeMapper.Deserialize(serializedPacketData);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
            Weight = packetData.Weight;
        }

        public double Weight { get; set; }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new RBNStatusPacket.PacketData()
            {
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
                Weight = Weight,
            });
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.RBNStatus, SerializePacketData());
        }

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public double Weight { get; set; }
        }
    }
}
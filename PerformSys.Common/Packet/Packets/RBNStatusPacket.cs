using PerformSys.Common.Packet.Packets.Data;
using PerformSys.Common.Utils;

namespace PerformSys.Common.Packet.Packets
{
    public class RBNStatusPacket : PacketBase, IPacket
    {
        public RBNStatusPacket(double weight)
        {
            Weight = weight;
        }

        public RBNStatusPacket(string serializedPacketData)
        {
            var packetData = SerializeMapper.Deserialize<RBNStatusPacketData>(serializedPacketData);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
            Weight = packetData.Weight;
        }

        public double Weight { get; set; }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new RBNStatusPacketData()
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
    }
}
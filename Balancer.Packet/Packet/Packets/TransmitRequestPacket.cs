using Balancer.Common.Packet.Packets.Data;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class TransmitRequestPacket : PacketBase, IPacket
    {
        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new TransmitRequestPacketData()
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
    }
}

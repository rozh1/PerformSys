using PerformSys.Common.Packet.Packets.Data;
using PerformSys.Common.Utils;

namespace PerformSys.Common.Packet.Packets
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

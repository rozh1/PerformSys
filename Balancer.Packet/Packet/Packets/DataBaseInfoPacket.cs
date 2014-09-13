using System;
using System.Collections.Generic;
using Balancer.Common.Packet.Packets.Data;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class DataBaseInfoPacket : PacketBase, IPacket
    {
        public DataBaseInfoPacket(Dictionary<string, UInt64> tableSizes)
        {
            TableSizes = tableSizes;
        }

        public DataBaseInfoPacket(string serializedPacketData)
        {
            var packetData = SerializeMapper.Deserialize<DataBaseInfoPacketData>(serializedPacketData);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
            TableSizes = packetData.TableSizes;
        }
        
        public Dictionary<string, UInt64> TableSizes { get; set; }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new DataBaseInfoPacketData()
            {
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
                TableSizes = TableSizes,
            });
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.DataBaseInfo, SerializePacketData());
        }
    }
}
﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class DataBaseInfoPacket : PacketBase, IPacket
    {
        public DataBaseInfoPacket(Dictionary<string,uint> tableSizes)
        {
            TableSizes = tableSizes;
        }

        public DataBaseInfoPacket(string serializedPacketData)
        {
            var packetData = (PacketData)SerializeMapper.Deserialize(serializedPacketData);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
            TableSizes = packetData.TableSizes;
        }

        public Dictionary<string,uint> TableSizes { get; set; }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new DataBaseInfoPacket.PacketData()
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

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public Dictionary<string, uint> TableSizes { get; set; }
        }
    }
}
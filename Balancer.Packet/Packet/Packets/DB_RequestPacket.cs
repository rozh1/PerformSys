using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class DbRequestPacket : IPacket
    {
        public DbRequestPacket(string query, int queryNumber)
        {
            Query = query;
            QueryNumber = queryNumber;
        }

        public DbRequestPacket(string query, int queryNumber, PacketBase packetBase)
        {
            Query = query;
            QueryNumber = queryNumber;
            ClientId = packetBase.ClientId;
            RegionId = packetBase.RegionId;
            GlobalId = packetBase.GlobalId;
        }

        public DbRequestPacket(string serializedQuery)
        {
            var packetData = (PacketData)SerializeMapper.Deserialize(serializedQuery);
            Query = packetData.Query;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new PacketData()
            {
                Query = Query,
                QueryNumber = QueryNumber,
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        public string Query { get; set; }
        public int QueryNumber { get; set; }

        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public uint ClientId { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, SerializePacketData());
        }

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public string Query { get; set; }
            [DataMember]
            public int QueryNumber { get; set; }
        }
    }
}
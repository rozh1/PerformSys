using System.Runtime.Serialization;

namespace Balancer.Common.Packet.Packets
{
    public class DbRequestPacket : IPacket
    {
        private string _serializedQuery;

        public DbRequestPacket(string query, int queryNumber)
        {
            _serializedQuery = SerializeMapper.Serialize(new PacketData()
            {
                Query = query,
                QueryNumber = queryNumber,
                ClientId = 0,
                RegionId = 0
            });
            Query = query;
            QueryNumber = queryNumber;
        }

        public DbRequestPacket(string query, int queryNumber, PacketBase packetBase)
        {
            _serializedQuery = SerializeMapper.Serialize(new PacketData()
            {
                Query = query,
                QueryNumber = queryNumber,
                ClientId = packetBase.ClientId,
                RegionId = packetBase.RegionId
            });
            Query = query;
            QueryNumber = queryNumber;
        }

        public DbRequestPacket(string serializedQuery)
        {
            _serializedQuery = serializedQuery;
            var packetData = (PacketData)SerializeMapper.Deserialize(serializedQuery);
            Query = packetData.Query;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
        }

        public string Query { get; set; }
        public int QueryNumber { get; set; }

        public uint RegionId { get; set; }
        public uint ClientId { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, _serializedQuery);
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
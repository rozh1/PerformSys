using System.Runtime.Serialization;

namespace Balancer.Common.Packet.Packets
{
    public class DbRequestPacket : IPacket
    {
        private string _serializedQuery;

        public DbRequestPacket(string query, int queryNumber)
        {
            _serializedQuery = SerializeMapper.Serialize(new PacketData() { Query = query, QueryNumber = queryNumber });
            Query = query;
            QueryNumber = queryNumber;
        }

        public DbRequestPacket(string serializedQuery)
        {
            _serializedQuery = serializedQuery;
            var packetData = (PacketData)SerializeMapper.Deserialize(serializedQuery);
            Query = packetData.Query;
            QueryNumber = packetData.QueryNumber;
        }

        public string Query { get; set; }
        public int QueryNumber { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, _serializedQuery);
        }

        [DataContract]
        private class PacketData
        {
            [DataMember]
            public string Query { get; set; }
            [DataMember]
            public int QueryNumber { get; set; }
        }
    }
}
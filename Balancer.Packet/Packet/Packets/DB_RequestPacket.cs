using Balancer.Common.Packet.Packets.Data;
using Balancer.Common.Utils;
using Balancer.Common.Utils.Interfaces;

namespace Balancer.Common.Packet.Packets
{
    public class DbRequestPacket : PacketBase, IPacket, ICloneable<DbRequestPacket>
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
            var packetData = SerializeMapper.Deserialize<DbRequestPacketData>(serializedQuery);
            Query = packetData.Query;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new DbRequestPacketData
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

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, SerializePacketData());
        }

        public DbRequestPacket Clone()
        {
            return new DbRequestPacket(Query, QueryNumber,
                new PacketBase
                {
                    ClientId = ClientId, 
                    GlobalId = GlobalId, 
                    RegionId = RegionId
                });
        }
    }
}
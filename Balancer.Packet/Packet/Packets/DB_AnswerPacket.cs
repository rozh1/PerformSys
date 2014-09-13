using Balancer.Common.Packet.Packets.Data;
using Balancer.Common.Utils;
using Balancer.Common.Utils.Interfaces;

namespace Balancer.Common.Packet.Packets
{
    public class DbAnswerPacket : PacketBase, IPacket, ICloneable<DbAnswerPacket>
    {
        public DbAnswerPacket(string answerPacketData)
        {
            var packetData = SerializeMapper.Deserialize<DbAnswerPacketData>(answerPacketData);
            AnswerDataTable = packetData.DataTable;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }

        public DbAnswerPacket(byte[] answer, int queryNumber, PacketBase packetBase)
        {
            AnswerDataTable = answer;
            QueryNumber = queryNumber;
            RegionId = packetBase.RegionId;
            ClientId = packetBase.ClientId;
            GlobalId = packetBase.GlobalId;
        }

        public byte[] AnswerDataTable { get; set; }
        public int QueryNumber { get; set; }
        
        public Packet GetPacket()
        {
            return new Packet(PacketType.Answer, SerializePacketData());
        }

        private string SerializePacketData()
        {
            return SerializeMapper.Serialize(new DbAnswerPacketData
            {
                DataTable = AnswerDataTable,
                QueryNumber = QueryNumber,
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        public DbAnswerPacket Clone()
        {
            return new DbAnswerPacket(AnswerDataTable, QueryNumber,
               new PacketBase
               {
                   ClientId = ClientId,
                   GlobalId = GlobalId,
                   RegionId = RegionId
               });
        }
    }
}
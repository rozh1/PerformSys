using System.Data;
using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class DbAnswerPacket : PacketBase, IPacket, IClonable<DbAnswerPacket>
    {
        public DbAnswerPacket(string answerPacketData)
        {
            var packetData = SerializeMapper.Deserialize<PacketData>(answerPacketData);
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
            return SerializeMapper.Serialize(new PacketData
            {
                DataTable = AnswerDataTable,
                QueryNumber = QueryNumber,
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        [DataContract]
        internal class PacketData : PacketBase
        {
            [DataMember]
            public byte[] DataTable { get; set; }

            [DataMember]
            public int QueryNumber { get; set; }
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
using System.Data;
using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class DbAnswerPacket : PacketBase, IPacket
    {
        public DbAnswerPacket(string answerPacketData)
        {
            var packetData = (PacketData) SerializeMapper.Deserialize(answerPacketData);
            AnswerDataTable = packetData.DataTable;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }

        public DbAnswerPacket(DataTable answer, int queryNumber, PacketBase packetBase)
        {
            AnswerDataTable = answer;
            QueryNumber = queryNumber;
            RegionId = packetBase.RegionId;
            ClientId = packetBase.ClientId;
            GlobalId = packetBase.GlobalId;
        }

        public DataTable AnswerDataTable { get; set; }
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
            public DataTable DataTable { get; set; }

            [DataMember]
            public int QueryNumber { get; set; }
        }
    }
}
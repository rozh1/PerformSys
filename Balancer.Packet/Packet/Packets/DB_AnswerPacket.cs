using System.Data;
using System.Runtime.Serialization;

namespace Balancer.Common.Packet.Packets
{
    public class DbAnswerPacket : IPacket
    {
        private string _answerString;

        public DbAnswerPacket(string answerPacketData)
        {
            _answerString = answerPacketData;
            var packetData = (PacketData)SerializeMapper.Deserialize(answerPacketData);
            AnswerDataTable = packetData.DataTable;
            QueryNumber = packetData.QueryNumber;
        }

        public DbAnswerPacket(DataTable answer, int queryNumber)
        {
            _answerString = SerializeMapper.Serialize(new PacketData() {DataTable = answer, QueryNumber = queryNumber});
            AnswerDataTable = answer;
        }

        public string AnswerString
        {
            get { return _answerString; }
            set { _answerString = value; }
        }

        public DataTable AnswerDataTable { get; set; }
        public int QueryNumber { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Answer, _answerString);
        }

        [DataContract]
        internal class PacketData
        {
            [DataMember]
            public DataTable DataTable { get; set; }
            [DataMember]
            public int QueryNumber { get; set; }
        }
    }
}
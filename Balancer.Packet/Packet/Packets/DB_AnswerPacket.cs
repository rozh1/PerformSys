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
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
        }

        public DbAnswerPacket(DataTable answer, int queryNumber, PacketBase packetBase)
        {
            _answerString =
                SerializeMapper.Serialize(new PacketData()
                {
                    DataTable = answer,
                    QueryNumber = queryNumber,
                    ClientId = packetBase.ClientId,
                    RegionId = packetBase.RegionId
                });
            AnswerDataTable = answer;
        }

        public string AnswerString
        {
            get { return _answerString; }
            set { _answerString = value; }
        }

        public DataTable AnswerDataTable { get; set; }
        public int QueryNumber { get; set; }

        public uint RegionId { get; set; }
        public uint ClientId { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Answer, _answerString);
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
using System.Data;

namespace Balancer.Packet.Packets
{
    public class DbAnswerPacket : IPacket
    {
        private string _answerString;

        public DbAnswerPacket(string answer)
        {
            _answerString = answer;
            AnswerDataTable = (DataTable)SerializeMapper.Deserialize(answer);
        }

        public DbAnswerPacket(DataTable answer)
        {
            _answerString = SerializeMapper.Serialize(answer);
            AnswerDataTable = answer;
        }

        public string AnswerString
        {
            get { return _answerString; }
            set { _answerString = value; }
        }

        public DataTable AnswerDataTable { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, _answerString);
        }
    }
}
namespace Balancer.Common.Packet.Packets
{
    public class DbRequestPacket : IPacket
    {
        private string _query;

        public DbRequestPacket(string query)
        {
            _query = query;
        }

        private string Query
        {
            get { return _query; }
            set { _query = value; }
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, _query);
        }
    }
}
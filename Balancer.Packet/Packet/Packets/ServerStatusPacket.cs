namespace Balancer.Common.Packet.Packets
{
    public class ServerStatusPacket : PacketBase, IPacket
    {
        private bool _status;

        public ServerStatusPacket(bool status)
        {
            _status = status;
        }

        public ServerStatusPacket(string packetData)
        {

            _status = packetData[0]=='1';
        }

        public bool Status
        {
            get { return _status; }
        }
        
        public Packet GetPacket()
        {
            return new Packet(PacketType.ServerStatus, (_status ? "1" : "0"));
        }
    }
}
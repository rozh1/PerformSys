namespace Balancer.Common.Packet.Packets
{
    public class StatusPacket : IPacket
    {
        private bool _status;

        public StatusPacket(bool status)
        {
            _status = status;
        }

        public StatusPacket(string packetData)
        {

            _status = packetData[0]=='1';
        }

        public bool Status
        {
            get { return _status; }
        }

        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public uint ClientId { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Status, (_status ? "1" : "0"));
        }
    }
}
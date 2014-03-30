﻿namespace Balancer.Packet.Packets
{
    public class StatusPacket : IPacket
    {
        private bool _status;

        public StatusPacket(bool status)
        {
            _status = status;
        }

        private bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Status, (_status ? "1" : "0"));
        }

        public bool GetStatus(Packet packet)
        {
            if (packet.Type != PacketType.Status) return false;
            if (packet.Data.Contains("1")) return true;
            return false;
        }
    }
}
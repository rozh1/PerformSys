using System;
using System.Text;

namespace Balancer.Common.Packet
{
    public class Packet
    {
        private String _data;
        private PacketType _type;
        private const string _packetEnd = "\n\n\r\r\t\t";

        public Packet(byte[] packet)
        {
            FromBytes(packet);
        }

        public Packet(string packet)
        {
            FromBase64String(packet);
        }

        public Packet(PacketType packetType, String packetData)
        {
            _type = packetType;
            _data = packetData;
        }

        public PacketType Type
        {
            get { return _type; }
        }

        public String Data
        {
            get { return _data; }
        }

        public static String PacketEnd
        {
            get { return _packetEnd; }
        }

        public String ToBase64String()
        {
            String str = ((int)_type).ToString("000") + _data;
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(strBytes) + _packetEnd;
        }

        private void FromBase64String(string base64String)
        {
            if (base64String.Length < 3)
            {
                _type = 0;
                _data = "0";
            }
            else
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(base64String);
                string str = Encoding.UTF8.GetString(base64EncodedBytes);
                _type = (PacketType) int.Parse(str.Substring(0, 3));
                _data = str.Substring(3);
            }
        }

        public Byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(ToBase64String());
        }

        private void FromBytes(byte[] bytes)
        {
            string str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            FromBase64String(str);
        }
    }
}
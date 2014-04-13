using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Balancer.Common.Packet
{
    public enum PacketType
    {
        Status,
        Request,
        Answer
    };

    public class Packet
    {
        private String _data;
        private PacketType _type;
        private string _id="0000";
        private string _clientId = "0000";
        private static string _packetEnd = "\n\n\r\r\t\t";

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
            _id = CreatePacketId(0, 0);
        }

        public Packet(PacketType packetType, String packetData, int globalId, int regionId, int clientId)
        {
            _type = packetType;
            _data = packetData;
            _id = CreatePacketId(globalId, regionId);
            _clientId = clientId.ToString("0000");
        }

        public Packet()
        {
        }

        public PacketType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public String Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public String ClientId
        {
            get { return _clientId; }
            set { _clientId = value; }
        }

        public static String PacketEnd
        {
            get { return _packetEnd; }
        }

        public String ToBase64String()
        {
            String str = ((int)_type).ToString("000") + _id + _clientId + _data;
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
                _id = str.Substring(3, 4);
                _clientId = str.Substring(7, 4);
                _data = str.Substring(11);
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

        public string CreatePacketId(int globalId, int regionId)
        {
            return globalId.ToString("00") + regionId.ToString("00");
        }

        /// <summary>
        /// численное представление ID из строки
        /// </summary>
        /// <param name="id">строка</param>
        /// <returns></returns>
        public int[] ParsePacketId(string id)
        {
            int[] result = new int[2];
            result[0] = int.Parse(id.Substring(0, 2));
            result[1] = int.Parse(id.Substring(2, 2));
            return result;
        }

    }

    public class SerializeMapper
    {
        public static string Serialize(object obj)
        {
            var serializer = new NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj);
                byte[] bytes = stream.ToArray();
                string xml = Encoding.UTF8.GetString(bytes);
                return xml;
            }
        }

        public static object Deserialize(string xml)
        {
            var serializer = new NetDataContractSerializer();
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            using (var stream = new MemoryStream(bytes))
            {
                object obj = serializer.Deserialize(stream);
                return obj;
            }
        }
    }
}
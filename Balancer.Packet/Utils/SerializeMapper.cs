using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Balancer.Common.Utils
{
    public static class SerializeMapper
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
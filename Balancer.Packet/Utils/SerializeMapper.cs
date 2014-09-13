using System;
using System.IO;

namespace Balancer.Common.Utils
{
    public static class SerializeMapper
    {
        public static string Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                byte[] bytes = stream.ToArray();
                string xml = Convert.ToBase64String(bytes);//Encoding.UTF8.GetString(bytes);
                return xml;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            byte[] bytes = Convert.FromBase64String(xml);//Encoding.UTF8.GetBytes(xml);
            using (var stream = new MemoryStream(bytes))
            {
                var obj = ProtoBuf.Serializer.Deserialize<T>(stream);
                return obj;
            }
        }
    }
}
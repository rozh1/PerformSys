using System;
using System.Data;
using System.IO;
using ProtoBuf;
using ProtoBuf.Data;

namespace Balancer.Common.Utils
{
    public static class SerializeMapper
    {
        public static string Serialize<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                if (typeof (T) == typeof (DataTable))
                {
                    DataTable dt = (DataTable) (object) obj;
                    DataSerializer.Serialize(stream, dt);
                }
                else
                {
                    Serializer.Serialize(stream, obj);
                }
                byte[] bytes = stream.ToArray();
                string xml = Convert.ToBase64String(bytes);
                return xml;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            byte[] bytes = Convert.FromBase64String(xml);
            using (var stream = new MemoryStream(bytes))
            {
                if (typeof (T) == typeof (DataTable))
                {
                    DataTable dt = new DataTable();
                    using (IDataReader reader = DataSerializer.Deserialize(stream))
                    {
                        dt.Load(reader);
                    }
                    return (T) (object) dt;
                }
                else
                {
                    var obj = Serializer.Deserialize<T>(stream);
                    return obj;
                }
            }
        }
    }
}
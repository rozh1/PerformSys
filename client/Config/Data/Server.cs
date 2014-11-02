using System.Xml.Serialization;

namespace client.Config.Data
{
    public class Server
    {
        [XmlAttribute]
        public string Host { get; set; }

        [XmlAttribute]
        public int Port { get; set; }
    }
}
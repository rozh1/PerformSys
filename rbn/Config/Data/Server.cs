using System.Xml.Serialization;

namespace rbn.Config.Data
{
    public class Server
    {
        [XmlAttribute]
        public uint Port { get; set; }
    }
}
using System.Xml.Serialization;

namespace server.Config.Data
{
    public class RBN
    {
        [XmlAttribute]
        public uint Port { get; set; }
        [XmlAttribute]
        public string Host { get; set; }
        [XmlAttribute]
        public int RegionId { get; set; }
    }
}
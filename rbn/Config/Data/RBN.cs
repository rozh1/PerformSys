using System.Xml.Serialization;

namespace rbn.Config.Data
{
    public class RBN
    {
        [XmlAttribute]
        public uint GlobalId { get; set; }
        [XmlAttribute]
        public uint RegionId { get; set; }
        [XmlAttribute]
        public uint Port { get; set; }
        [XmlAttribute]
        public int ServersCount { get; set; }
        [XmlAttribute]
        public int MaxServersCount { get; set; }
    }
}

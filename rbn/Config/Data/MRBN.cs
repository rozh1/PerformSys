using System.Xml.Serialization;

namespace rbn.Config.Data
{
    public class MRBN
    {
        [XmlAttribute]
        public string Host { get; set; }
        [XmlAttribute]
        public uint Port { get; set; }
    }
}
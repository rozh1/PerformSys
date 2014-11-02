using System.Xml.Serialization;

namespace client.Config.Data
{
    public class QueryConfig
    {
        [XmlAttribute]
        public int Number { get; set; }
    }
}
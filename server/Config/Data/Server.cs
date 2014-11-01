using System.Xml.Serialization;

namespace server.Config.Data
{
    public class Server
    {
        public RBN RBN { get; set; }
        [XmlAttribute]
        public WorkMode WorkMode { get; set; }
    }
}
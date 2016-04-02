using System.Xml.Serialization;

namespace router.Config.Data
{
    public class Router
    {
        public RBN RBN { get; set; }
        [XmlAttribute]
        public int Port { get; set; }
    }
}
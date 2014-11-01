using System.Xml.Serialization;

namespace server.Config.Data
{
    public class DataBase
    {
        [XmlAttribute]
        public int RegionId { get; set; }
        [XmlAttribute]
        public string Host { get; set; }
        [XmlAttribute]
        public uint Port { get; set; }
        [XmlAttribute]
        public string UserName { get; set; }
        [XmlAttribute]
        public string Password { get; set; }
        [XmlAttribute]
        public string DataBaseName { get; set; }

        /// <summary>
        ///     Конфиг времен выполения запросов для режима симуляции
        /// </summary>
        public Data.SimulationParams SimulationParams { get; set; }

        /// <summary>
        ///     Конфиг размеров таблиц для симуляции
        /// </summary>
        public Data.SimulationSizes SimulationSizes { get; set; }
    }
}
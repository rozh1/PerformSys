using System.Xml.Serialization;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Logger.Interfaces;

namespace server.Config.Data
{
    public class Log : ILogConfig
    {
        public string LogFile { get; set; }
        public string StatsFile { get; set; }
        public string QueueStatsFile { get; set; }
        public string LogDir { get; set; }

        [XmlAttribute]
        public bool WriteToConsole { get; set; }
        [XmlAttribute]
        public LogLevel LogLevel { get; set; }
        [XmlAttribute]
        public LogMode LogMode { get; set; }
    }
}

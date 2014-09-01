﻿using Balancer.Common.Logger.Enums;
using Balancer.Common.Logger.Interfaces;

namespace client.Config.Data
{
    public class Log : ILogConfig
    {
        public string LogFile { get; set; }
        public string StatsFile { get; set; }
        public LogLevel LogLevel { get; set; }
        public LogMode LogMode { get; set; }
        public string LogDir { get; set; }
        public bool WriteToConsole { get; set; }
    }
}
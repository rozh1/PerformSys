using Balancer.Common.Logger.Enums;
using rbn.Config.Data;

namespace rbn.Config
{
    class ConfigInit
    {
        public ConfigInit()
        {
            Config = new RBNConfig
            {
                Server = new Data.Server()
                {
                    Port = 3410
                },
                RBN = new RBN()
                {
                    GlobalId = 1,
                    RegionId = 1,
                    Port = 3409,
                    MaxServersCount = 1,
                    ServersCount = 1
                },
                MRBN = new MRBN()
                {
                    Host = "localhost",
                    Port = 3401
                },
                Log = new Log()
                {
                    LogFile = "rbnLog.txt",
                    StatsFile = "rbnStats.csv",
                    QueueStatsFile = "rbnQueue.csv",
                    LogLevel = LogLevel.DEBUG,
                    LogMode = LogMode.MULTIPLE,
                    WriteToConsole = true
                }
            };
        }

        public RBNConfig Config { get; private set; }
    }
}

using PerformSys.Common.Logger.Enums;
using router.Config.Data;

namespace router.Config
{
    internal class ConfigInit
    {
        public ConfigInit()
        {
            Config = new RouterConfig
            {
                Router = new router.Config.Data.Router
                {
                    RBN = new RBN
                    {
                        Host = "localhost",
                        Port = 3410,
                        RegionId = 1,
                    },
                    Port = 3411
                },
                Log = new Log
                {
                    LogFile = "routerLog.txt",
                    QueueStatsFile = "routerQueue.csv",
                    StatsFile = "routerStats.csv",
                    LogDir = "",
                    WriteToConsole = true,
                    LogLevel = LogLevel.DEBUG,
                    LogMode = LogMode.MULTIPLE
                }
            };
        }

        public RouterConfig Config { get; private set; }
    }
}
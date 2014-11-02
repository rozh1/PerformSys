using System;
using Balancer.Common.Logger.Enums;
using client.Config.Data;

namespace client.Config
{
    internal class ConfigInit
    {
        public ConfigInit()
        {
            Config = new ClientConfig
            {
                Server = new Server
                {
                    Host = "localhost",
                    Port = 3409,
                },
                Log = new Log
                {
                    LogFile = "clientLog.txt",
                    StatsFile = "clientStats.csv",
                    LogDir = "",
                    WriteToConsole = true,
                    LogLevel = LogLevel.DEBUG,
                    LogMode = LogMode.MULTIPLE
                },
                QuerySequence = new Data.QuerySequence
                {
                    Mode = QuerySequenceMode.FromList,
                    List = new[]
                    {
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 2},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 5},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 8}
                    }
                },
                Scenario = new Scenario
                {
                    ClientCount = 14,
                    StartTime = DateTime.Now,
                    ScenarioSteps = new[]
                    {
                        new ScenarioStep
                        {
                            Action = ScenarioActions.Work,
                            Duration = new TimeSpan(0, 0, 0, 1)
                        }
                    }
                }
            };
        }

        public ClientConfig Config { get; private set; }
    }
}
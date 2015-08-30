using Balancer.Common.Logger.Enums;
using server.Config.Data;

namespace server.Config
{
    internal class ConfigInit
    {
        public ConfigInit()
        {
            Config = new ServerConfig
            {
                DataBase = new[]
                {
                    new Data.DataBase
                    {
                        DataBaseName = "tpch1",
                        Host = "localhost",
                        Password = "0000",
                        UserName = "root",
                        Port = 3306,
                        RegionId = 1,
                        GetFullQueryResult = true,
                        SimulationParams =
                            new SimulationParams
                            {
                                {1, new[] {11754, 4622}},
                                {2, new[] {571, 52018}},
                                {3, new[] {2287, 4654}},
                                {4, new[] {869, 2646}},
                                {5, new[] {2132, 2498}},
                                {6, new[] {1814, 1790}},
                                {7, new[] {2879, 2846}},
                                {8, new[] {4888, 2042}},
                                {9, new[] {16158, 35154}},
                                {10, new[] {2783, 12658}},
                                {11, new[] {152, 1666}},
                                {12, new[] {4929, 2286}},
                                {13, new[] {3285, 7590}},
                                {14, new[] {10318, 1814}},
                            },
                        SimulationSizes =
                            new SimulationSizes
                            {
                                {"customer", 28884992},
                                {"lineitem", 870318080},
                                {"nation", 16384},
                                {"orders", 193658880},
                                {"part", 32047104},
                                {"partsupp", 141213696},
                                {"region", 16384},
                                {"supplier", 2637824},
                            },
                    }, 
                    new Data.DataBase
                    {
                        DataBaseName = "tpch3",
                        Host = "localhost",
                        Password = "0000",
                        UserName = "root",
                        Port = 3306,
                        RegionId = 2,
                        GetFullQueryResult = true,
                        SimulationParams =
                            new SimulationParams
                            {
                                {1, new[] {38412, 4622 }},
                                {2, new[] {14538, 740066}},
                                {3, new[] {4757, 9605286}},
                                {4, new[] {5217, 2630 }},
                                {5, new[] {2485, 2498}},
                                {6, new[] {5853, 1778}},
                                {7, new[] {17878, 2846}},
                                {8, new[] {10194, 2042}},
                                {9, new[] {60784, 35154}},
                                {10, new[] {16068, 59003658}},
                                {11, new[] {400, 1666}},
                                {12, new[] {29774, 2278}},
                                {13, new[] {8382, 7702}},
                                {14, new[] {41704, 1814}},
                            },
                        SimulationSizes =
                            new SimulationSizes
                            {
                                {"customer", 86605824},
                                {"lineitem", 2445279232},
                                {"nation",  16384},
                                {"orders", 565166080},
                                {"part", 97107968},
                                {"partsupp", 421494784},
                                {"region", 16384},
                                {"supplier", 5783552},
                            },
                    }
                },
                Server = new Data.Server
                {
                    RBN = new RBN
                    {
                        Host = "localhost",
                        Port = 3410,
                        RegionId = 1,
                    },
                    WorkMode = WorkMode.Simulation
                },
                Log = new Log
                {
                    LogFile = "serverLog.txt",
                    QueueStatsFile = "serverQueue.csv",
                    StatsFile = "serverStats.csv",
                    LogDir = "",
                    WriteToConsole = true,
                    LogLevel = LogLevel.DEBUG,
                    LogMode = LogMode.MULTIPLE
                }
            };
        }

        public ServerConfig Config { get; private set; }
    }
}
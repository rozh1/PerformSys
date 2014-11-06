#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
#endregion

﻿using System;
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
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 2},
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 9},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 5},
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 2},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 9},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 13},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 11},
                        new QueryConfig {Number = 13},
                        new QueryConfig {Number = 13},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 5},
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 5},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 9},
                        new QueryConfig {Number = 9},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 11},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 13},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 13},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 2},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 4},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 13},
                        new QueryConfig {Number = 9},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 8},
                        new QueryConfig {Number = 14},
                        new QueryConfig {Number = 1},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 5},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 7},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 10},
                        new QueryConfig {Number = 12},
                        new QueryConfig {Number = 3},
                        new QueryConfig {Number = 9},
                        new QueryConfig {Number = 6},
                        new QueryConfig {Number = 4}
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
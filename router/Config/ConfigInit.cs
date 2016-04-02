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

﻿using Balancer.Common.Logger.Enums;
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
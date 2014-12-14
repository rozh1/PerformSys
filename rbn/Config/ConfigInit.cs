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
                    Port = 3401,
                    UseMRBN = false
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

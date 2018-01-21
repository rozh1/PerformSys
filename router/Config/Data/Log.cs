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

﻿using System.Xml.Serialization;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Logger.Interfaces;

namespace router.Config.Data
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

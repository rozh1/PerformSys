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

namespace server.Config.Data
{
    public class DataBase
    {
        [XmlAttribute]
        public int RegionId { get; set; }
        [XmlAttribute]
        public string Host { get; set; }
        [XmlAttribute]
        public uint Port { get; set; }
        [XmlAttribute]
        public string UserName { get; set; }
        [XmlAttribute]
        public string Password { get; set; }
        [XmlAttribute]
        public string DataBaseName { get; set; }

        /// <summary>
        ///     Конфиг времен выполения запросов для режима симуляции
        /// </summary>
        public Data.SimulationParams SimulationParams { get; set; }

        /// <summary>
        ///     Конфиг размеров таблиц для симуляции
        /// </summary>
        public Data.SimulationSizes SimulationSizes { get; set; }
    }
}
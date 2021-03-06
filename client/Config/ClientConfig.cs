﻿#region Copyright
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
using System.Collections.Generic;
using System.Xml.Serialization;
using client.Config.Data;
using PerformSys.Common.Config;

namespace client.Config
{
    public class ClientConfig : ConfigBase<ClientConfig>
    {
        /// <summary>
        ///     Конфиг сервера
        /// </summary>
        public Server Server { get; set; }

        /// <summary>
        ///     Конфиг лога
        /// </summary>
        public Log Log { get; set; }

        public Data.QuerySequence QuerySequence { get; set; }

        /// <summary>
        ///     Сценарий работы клиентов
        /// </summary>
        public Scenario Scenario { get; set; }

        /// <summary>
        ///     Список возможных заросов
        /// </summary>
        [XmlIgnore]
        public List<QueryConfig> Queries { get; set; }
    }
}
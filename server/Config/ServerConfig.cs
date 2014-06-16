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
using System.IO;
using System.Xml.Serialization;
using Balancer.Common;

namespace server.Config
{
    public class ServerConfig
    {
        [XmlIgnore] public static ServerConfig Instance;

        /// <summary>
        ///     Конфиг базы данных
        /// </summary>
        public Data.DataBase DataBase { get; set; }

        /// <summary>
        ///     Конфиг базы данных
        /// </summary>
        public Data.Server Server { get; set; }

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof (ServerConfig));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static ServerConfig Load(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static ServerConfig Load(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof (ServerConfig));
            try
            {
                Instance = serializer.Deserialize(fileStream) as ServerConfig;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
            }
            return Instance;
        }
    }
}
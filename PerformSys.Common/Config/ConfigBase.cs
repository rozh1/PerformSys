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
using System;
using System.IO;
using System.Xml.Serialization;

namespace PerformSys.Common.Config
{
    /// <summary>
    /// Базовый класс конфигурации
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigBase<T>
    {
        [XmlIgnore]
        public static T Instance;

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static T Load(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return Load(stream);
            }
        }

        public static T Load(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                Instance = (T)serializer.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Instance;
        }
    }
}

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

﻿using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Balancer.Common.Utils
{
    public static class SerializeMapper
    {
        public static string Serialize(object obj)
        {
            var serializer = new NetDataContractSerializer();
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj);
                byte[] bytes = stream.ToArray();
                string xml = Encoding.UTF8.GetString(bytes);
                return xml;
            }
        }

        public static object Deserialize(string xml)
        {
            var serializer = new NetDataContractSerializer();
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            using (var stream = new MemoryStream(bytes))
            {
                object obj = serializer.Deserialize(stream);
                return obj;
            }
        }
    }
}
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
using System.Data;
using System.IO;
using ProtoBuf;
using ProtoBuf.Data;

namespace Balancer.Common.Utils
{
    public static class SerializeMapper
    {
        public static string Serialize<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                if (typeof (T) == typeof (DataTable))
                {
                    DataTable dt = (DataTable) (object) obj;
                    DataSerializer.Serialize(stream, dt);
                }
                else
                {
                    Serializer.Serialize(stream, obj);
                }
                byte[] bytes = stream.ToArray();
                string xml = Convert.ToBase64String(bytes); //Encoding.UTF8.GetString(bytes);
                return xml;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            byte[] bytes = Convert.FromBase64String(xml); //Encoding.UTF8.GetBytes(xml);
            using (var stream = new MemoryStream(bytes))
            {
                if (typeof (T) == typeof (DataTable))
                {
                    DataTable dt = new DataTable();
                    using (IDataReader reader = DataSerializer.Deserialize(stream))
                    {
                        dt.Load(reader);
                    }
                    return (T) (object) dt;
                }
                else
                {
                    var obj = Serializer.Deserialize<T>(stream);
                    return obj;
                }
            }
        }
    }
}
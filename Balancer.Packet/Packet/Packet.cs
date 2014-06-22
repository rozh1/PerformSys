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
using System.Text;

namespace Balancer.Common.Packet
{
    public class Packet
    {
        private String _data;
        private PacketType _type;
        private const string _packetEnd = "\n\n\r\r\t\t";

        public Packet(byte[] packet)
        {
            FromBytes(packet);
        }

        public Packet(string packet)
        {
            FromBase64String(packet);
        }

        public Packet(PacketType packetType, String packetData)
        {
            _type = packetType;
            _data = packetData;
        }

        public PacketType Type
        {
            get { return _type; }
        }

        public String Data
        {
            get { return _data; }
        }

        public static String PacketEnd
        {
            get { return _packetEnd; }
        }

        public String ToBase64String()
        {
            String str = ((int)_type).ToString("000") + _data;
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(strBytes) + _packetEnd;
        }

        private void FromBase64String(string base64String)
        {
            if (base64String.Length < 3)
            {
                _type = 0;
                _data = "0";
            }
            else
            {
                byte[] base64EncodedBytes = Convert.FromBase64String(base64String);
                string str = Encoding.UTF8.GetString(base64EncodedBytes);
                _type = (PacketType) int.Parse(str.Substring(0, 3));
                _data = str.Substring(3);
            }
        }

        public Byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(ToBase64String());
        }

        private void FromBytes(byte[] bytes)
        {
            string str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            FromBase64String(str);
        }
    }
}
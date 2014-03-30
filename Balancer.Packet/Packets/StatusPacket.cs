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

﻿namespace Balancer.Packet.Packets
{
    public class StatusPacket : IPacket
    {
        private bool _status;

        public StatusPacket(bool status)
        {
            _status = status;
        }

        private bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Status, (_status ? "1" : "0"));
        }

        public bool GetStatus(Packet packet)
        {
            if (packet.Type != PacketType.Status) return false;
            if (packet.Data.Contains("1")) return true;
            return false;
        }
    }
}
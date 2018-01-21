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
using ProtoBuf;

namespace PerformSys.Common.Packet.Packets.Data
{
    [ProtoContract]
    public class DbRequestPacketData : PacketBase
    {
        [ProtoMember(1)]
        public string Query { get; set; }

        [ProtoMember(2)]
        public int QueryNumber { get; set; }
    }
}
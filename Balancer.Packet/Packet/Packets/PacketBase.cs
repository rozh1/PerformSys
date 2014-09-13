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

﻿using Balancer.Common.Packet.Packets.Data;
using Balancer.Common.Utils;
using ProtoBuf;

namespace Balancer.Common.Packet.Packets
{
    [ProtoContract]
    [ProtoInclude(100, typeof(DataBaseInfoPacketData))]
    [ProtoInclude(200, typeof(DbAnswerPacketData))]
    [ProtoInclude(300, typeof(DbRequestPacketData))]
    public class PacketBase
    {
        [ProtoMember(1)]
        public uint GlobalId { get; set; }
        [ProtoMember(2)]
        public uint RegionId { get; set; }
        [ProtoMember(3)]
        public uint ClientId { get; set; }

        public void Deserialize(string serializedPacket)
        {
            var packetData = SerializeMapper.Deserialize<PacketBase>(serializedPacket);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }
    }
}

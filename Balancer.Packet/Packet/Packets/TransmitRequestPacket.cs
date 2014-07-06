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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class TransmitRequestPacket : PacketBase, IPacket
    {
        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new PacketData()
            {
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.TransmitRequest, SerializePacketData());
        }

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public double Weight { get; set; }
        }
    }
}
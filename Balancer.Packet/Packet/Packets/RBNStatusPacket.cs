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

﻿using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class RBNStatusPacket : PacketBase, IPacket
    {
        public RBNStatusPacket(double weight)
        {
            Weight = weight;
        }

        public RBNStatusPacket(string serializedPacketData)
        {
            var packetData = (PacketData)SerializeMapper.Deserialize(serializedPacketData);
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
            Weight = packetData.Weight;
        }

        public double Weight { get; set; }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new RBNStatusPacket.PacketData()
            {
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
                Weight = Weight,
            });
        }

        public Packet GetPacket()
        {
            return new Packet(PacketType.RBNStatus, SerializePacketData());
        }

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public double Weight { get; set; }
        }
    }
}
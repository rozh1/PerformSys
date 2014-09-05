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
using Balancer.Common.Utils.Interfaces;

namespace Balancer.Common.Packet.Packets
{
    public class DbRequestPacket : PacketBase, IPacket, ICloneable<DbRequestPacket>
    {
        public DbRequestPacket(string query, int queryNumber)
        {
            Query = query;
            QueryNumber = queryNumber;
        }

        public DbRequestPacket(string query, int queryNumber, PacketBase packetBase)
        {
            Query = query;
            QueryNumber = queryNumber;
            ClientId = packetBase.ClientId;
            RegionId = packetBase.RegionId;
            GlobalId = packetBase.GlobalId;
        }

        public DbRequestPacket(string serializedQuery)
        {
            var packetData = SerializeMapper.Deserialize<PacketData>(serializedQuery);
            Query = packetData.Query;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }

        string SerializePacketData()
        {
            return SerializeMapper.Serialize(new PacketData
            {
                Query = Query,
                QueryNumber = QueryNumber,
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        public string Query { get; set; }
        public int QueryNumber { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Request, SerializePacketData());
        }

        [DataContract]
        private class PacketData : PacketBase
        {
            [DataMember]
            public string Query { get; set; }
            [DataMember]
            public int QueryNumber { get; set; }
        }

        public DbRequestPacket Clone()
        {
            return new DbRequestPacket(Query, QueryNumber,
                new PacketBase
                {
                    ClientId = ClientId, 
                    GlobalId = GlobalId, 
                    RegionId = RegionId
                });
        }
    }
}
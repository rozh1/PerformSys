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

﻿using System.Data;
using System.Runtime.Serialization;
using Balancer.Common.Utils;

namespace Balancer.Common.Packet.Packets
{
    public class DbAnswerPacket : PacketBase, IPacket
    {
        public DbAnswerPacket(string answerPacketData)
        {
            var packetData = (PacketData) SerializeMapper.Deserialize(answerPacketData);
            AnswerDataTable = packetData.DataTable;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
            GlobalId = packetData.GlobalId;
        }

        public DbAnswerPacket(DataTable answer, int queryNumber, PacketBase packetBase)
        {
            AnswerDataTable = answer;
            QueryNumber = queryNumber;
            RegionId = packetBase.RegionId;
            ClientId = packetBase.ClientId;
            GlobalId = packetBase.GlobalId;
        }

        public DataTable AnswerDataTable { get; set; }
        public int QueryNumber { get; set; }
        
        public Packet GetPacket()
        {
            return new Packet(PacketType.Answer, SerializePacketData());
        }

        private string SerializePacketData()
        {
            return SerializeMapper.Serialize(new PacketData
            {
                DataTable = AnswerDataTable,
                QueryNumber = QueryNumber,
                ClientId = ClientId,
                RegionId = RegionId,
                GlobalId = GlobalId,
            });
        }

        [DataContract]
        internal class PacketData : PacketBase
        {
            [DataMember]
            public DataTable DataTable { get; set; }

            [DataMember]
            public int QueryNumber { get; set; }
        }
    }
}
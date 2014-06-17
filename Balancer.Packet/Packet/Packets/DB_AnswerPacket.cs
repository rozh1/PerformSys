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

namespace Balancer.Common.Packet.Packets
{
    public class DbAnswerPacket : IPacket
    {
        private string _answerString;

        public DbAnswerPacket(string answerPacketData)
        {
            _answerString = answerPacketData;
            var packetData = (PacketData)SerializeMapper.Deserialize(answerPacketData);
            AnswerDataTable = packetData.DataTable;
            QueryNumber = packetData.QueryNumber;
            RegionId = packetData.RegionId;
            ClientId = packetData.ClientId;
        }

        public DbAnswerPacket(DataTable answer, int queryNumber, PacketBase packetBase)
        {
            _answerString =
                SerializeMapper.Serialize(new PacketData()
                {
                    DataTable = answer,
                    QueryNumber = queryNumber,
                    ClientId = packetBase.ClientId,
                    RegionId = packetBase.RegionId
                });
            AnswerDataTable = answer;
        }

        public string AnswerString
        {
            get { return _answerString; }
            set { _answerString = value; }
        }

        public DataTable AnswerDataTable { get; set; }
        public int QueryNumber { get; set; }

        public uint RegionId { get; set; }
        public uint ClientId { get; set; }

        public Packet GetPacket()
        {
            return new Packet(PacketType.Answer, _answerString);
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
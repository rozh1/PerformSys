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
using Balancer.Common.Packet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balancer.Common.Test.Packet
{
    [TestClass]
    public class DbAnswerPacketTest
    {
        [TestMethod]
        public void DbAnswerPacketSeralizationTest()
        {
            DataTable dt = new DataTable();
            dt.TableName = "randomTable";
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            PacketBase packetBase = new PacketBase()
            {
                ClientId = 1,
                GlobalId = 2,
                RegionId = 3
            };

            DbAnswerPacket dbAnswerPacket = new DbAnswerPacket(dt, 1, packetBase);

            Balancer.Common.Packet.Packet packet = dbAnswerPacket.GetPacket();

            DbAnswerPacket dbAnswerPacketDeserialized = new DbAnswerPacket(packet.Data);

            Assert.AreEqual(dbAnswerPacketDeserialized.QueryNumber, dbAnswerPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbAnswerPacketDeserialized.ClientId, dbAnswerPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbAnswerPacketDeserialized.RegionId, dbAnswerPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbAnswerPacketDeserialized.GlobalId, dbAnswerPacket.GlobalId, "Разные глобальные номера");
            Assert.IsTrue(Util.CustomComparators.AreTablesEqual(dbAnswerPacketDeserialized.AnswerDataTable, dbAnswerPacket.AnswerDataTable), "Разные данные в ответе");
        }
    }
}

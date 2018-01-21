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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerformSys.Common.Packet.Packets;

namespace PerformSys.Common.Test.Packet
{
    [TestClass]
    public class DbRequestPacketTest
    {
        [TestMethod]
        public void DbRequestPacketSeralizationTest()
        {
            var dbRequestPacket = new DbRequestPacket("query", 1);

            PerformSys.Common.Packet.Packet packet = dbRequestPacket.GetPacket();

            var dbRequestPacketDeserialized = new DbRequestPacket(packet.Data);

            Assert.AreEqual(dbRequestPacketDeserialized.QueryNumber, dbRequestPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbRequestPacketDeserialized.ClientId, dbRequestPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbRequestPacketDeserialized.RegionId, dbRequestPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbRequestPacketDeserialized.GlobalId, dbRequestPacket.GlobalId, "Разные глобальные номера");
            Assert.AreEqual(dbRequestPacketDeserialized.Query, dbRequestPacket.Query, "Разные запросы в ответе");
        }

        [TestMethod]
        public void DbRequestPacketWithPacketBaseSeralizationTest()
        {
            var packetBase = new PacketBase
            {
                ClientId = 1,
                GlobalId = 2,
                RegionId = 3
            };

            var dbRequestPacket = new DbRequestPacket("query", 1, packetBase);

            PerformSys.Common.Packet.Packet packet = dbRequestPacket.GetPacket();

            var dbRequestPacketDeserialized = new DbRequestPacket(packet.Data);

            Assert.AreEqual(dbRequestPacketDeserialized.QueryNumber, dbRequestPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbRequestPacketDeserialized.ClientId, dbRequestPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbRequestPacketDeserialized.RegionId, dbRequestPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbRequestPacketDeserialized.GlobalId, dbRequestPacket.GlobalId, "Разные глобальные номера");
            Assert.AreEqual(dbRequestPacketDeserialized.Query, dbRequestPacket.Query, "Разные запросы в ответе");
        }
    }
}
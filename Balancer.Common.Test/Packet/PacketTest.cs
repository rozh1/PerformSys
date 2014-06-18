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
using Balancer.Common.Packet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balancer.Common.Test.Packet
{
    [TestClass]
    public class PacketTest
    {
        [TestMethod]
        public void PacketSeralizationToBytesTest()
        {
            StatusPacket statusPacket = new StatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            byte[] bytes = packet.ToBytes();

            Common.Packet.Packet recivedPacket = new Common.Packet.Packet(bytes);

            Assert.AreEqual(packet.Type, recivedPacket.Type, "Разные типы");

            StatusPacket statusPacketDeserialized = new StatusPacket(recivedPacket.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, statusPacket.Status, "Разные статусы");
            Assert.AreEqual(statusPacketDeserialized.ClientId, statusPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(statusPacketDeserialized.RegionId, statusPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, statusPacket.GlobalId, "Разные глобальные номера");
        }

        [TestMethod]
        public void PacketSeralizationToStringTest()
        {
            StatusPacket statusPacket = new StatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            string base64String  = packet.ToBase64String();

            Common.Packet.Packet recivedPacket = new Common.Packet.Packet(base64String);

            Assert.AreEqual(packet.Type, recivedPacket.Type, "Разные типы");

            StatusPacket statusPacketDeserialized = new StatusPacket(recivedPacket.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, statusPacket.Status, "Разные статусы");
            Assert.AreEqual(statusPacketDeserialized.ClientId, statusPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(statusPacketDeserialized.RegionId, statusPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, statusPacket.GlobalId, "Разные глобальные номера");
        }

        [TestMethod]
        public void PacketBadSeralizationToStringTest()
        {
            Common.Packet.Packet recivedPacket = new Common.Packet.Packet("12");

            Assert.AreEqual(Common.Packet.PacketType.Status, recivedPacket.Type, "Тип должен быть преобразован в статус");

            StatusPacket statusPacketDeserialized = new StatusPacket(recivedPacket.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, false, "Пакет должен быть статусом не готовности");
            Assert.AreEqual(statusPacketDeserialized.ClientId, (uint)0, "Номер клиента должен быть 0");
            Assert.AreEqual(statusPacketDeserialized.RegionId, (uint)0, "Номер региона должен быть 0");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, (uint)0, "Глобавльный номер должен быть 0");
        }

        [TestMethod]
        public void PacketEndTest()
        {
            StatusPacket statusPacket = new StatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            Assert.IsTrue(packet.ToBase64String().Contains(Common.Packet.Packet.PacketEnd));
        }
    }
}

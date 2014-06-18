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
    public class StatusPacketTest
    {
        [TestMethod]
        public void StatusPacketSeralizationTest()
        {
            StatusPacket statusPacket = new StatusPacket(true);

            Balancer.Common.Packet.Packet packet = statusPacket.GetPacket();

            StatusPacket statusPacketDeserialized = new StatusPacket(packet.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, statusPacket.Status, "Разные статусы");
            Assert.AreEqual(statusPacketDeserialized.ClientId, statusPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(statusPacketDeserialized.RegionId, statusPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, statusPacket.GlobalId, "Разные глобальные номера");
        }
    }
}

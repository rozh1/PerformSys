using System;
using System.Data;
using Balancer.Common.Packet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balancer.Common.Test.Packet
{
    [TestClass]
    public class DbRequestPacketTest
    {
        [TestMethod]
        public void DbRequestPacketSeralizationTest()
        {
            DbRequestPacket dbRequestPacket = new DbRequestPacket("query", 1);

            Balancer.Common.Packet.Packet packet = dbRequestPacket.GetPacket();

            DbRequestPacket dbRequestPacketDeserialized = new DbRequestPacket(packet.Data);

            Assert.AreEqual(dbRequestPacketDeserialized.QueryNumber, dbRequestPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbRequestPacketDeserialized.ClientId, dbRequestPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbRequestPacketDeserialized.RegionId, dbRequestPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbRequestPacketDeserialized.GlobalId, dbRequestPacket.GlobalId, "Разные глобальные номера");
            Assert.AreEqual(dbRequestPacketDeserialized.Query,dbRequestPacket.Query, "Разные запросы в ответе");
        }

        [TestMethod]
        public void DbRequestPacketWithPacketBaseSeralizationTest()
        {
            PacketBase packetBase = new PacketBase()
            {
                ClientId = 1,
                GlobalId = 2,
                RegionId = 3
            };

            DbRequestPacket dbRequestPacket = new DbRequestPacket("query", 1, packetBase);

            Balancer.Common.Packet.Packet packet = dbRequestPacket.GetPacket();

            DbRequestPacket dbRequestPacketDeserialized = new DbRequestPacket(packet.Data);

            Assert.AreEqual(dbRequestPacketDeserialized.QueryNumber, dbRequestPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbRequestPacketDeserialized.ClientId, dbRequestPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbRequestPacketDeserialized.RegionId, dbRequestPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbRequestPacketDeserialized.GlobalId, dbRequestPacket.GlobalId, "Разные глобальные номера");
            Assert.AreEqual(dbRequestPacketDeserialized.Query, dbRequestPacket.Query, "Разные запросы в ответе");
        }
    }
}

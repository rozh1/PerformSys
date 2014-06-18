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
            var dbRequestPacket = new DbRequestPacket("query", 1);

            Common.Packet.Packet packet = dbRequestPacket.GetPacket();

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

            Common.Packet.Packet packet = dbRequestPacket.GetPacket();

            var dbRequestPacketDeserialized = new DbRequestPacket(packet.Data);

            Assert.AreEqual(dbRequestPacketDeserialized.QueryNumber, dbRequestPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbRequestPacketDeserialized.ClientId, dbRequestPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbRequestPacketDeserialized.RegionId, dbRequestPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbRequestPacketDeserialized.GlobalId, dbRequestPacket.GlobalId, "Разные глобальные номера");
            Assert.AreEqual(dbRequestPacketDeserialized.Query, dbRequestPacket.Query, "Разные запросы в ответе");
        }
    }
}
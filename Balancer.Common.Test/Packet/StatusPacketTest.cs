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
            var statusPacket = new ServerStatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            var statusPacketDeserialized = new ServerStatusPacket(packet.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, statusPacket.Status, "Разные статусы");
            Assert.AreEqual(statusPacketDeserialized.ClientId, statusPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(statusPacketDeserialized.RegionId, statusPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, statusPacket.GlobalId, "Разные глобальные номера");
        }
    }
}
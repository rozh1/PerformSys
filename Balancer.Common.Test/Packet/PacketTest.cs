using Balancer.Common.Packet;
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
            var statusPacket = new ServerStatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            byte[] bytes = packet.ToBytes();

            var recivedPacket = new Common.Packet.Packet(bytes);

            Assert.AreEqual(packet.Type, recivedPacket.Type, "Разные типы");

            var statusPacketDeserialized = new ServerStatusPacket(recivedPacket.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, statusPacket.Status, "Разные статусы");
            Assert.AreEqual(statusPacketDeserialized.ClientId, statusPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(statusPacketDeserialized.RegionId, statusPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, statusPacket.GlobalId, "Разные глобальные номера");
        }

        [TestMethod]
        public void PacketSeralizationToStringTest()
        {
            var statusPacket = new ServerStatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            string base64String = packet.ToBase64String();

            var recivedPacket = new Common.Packet.Packet(base64String);

            Assert.AreEqual(packet.Type, recivedPacket.Type, "Разные типы");

            var statusPacketDeserialized = new ServerStatusPacket(recivedPacket.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, statusPacket.Status, "Разные статусы");
            Assert.AreEqual(statusPacketDeserialized.ClientId, statusPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(statusPacketDeserialized.RegionId, statusPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, statusPacket.GlobalId, "Разные глобальные номера");
        }

        [TestMethod]
        public void PacketBadSeralizationToStringTest()
        {
            var recivedPacket = new Common.Packet.Packet("12");

            Assert.AreEqual(PacketType.ServerStatus, recivedPacket.Type, "Тип должен быть преобразован в статус");

            var statusPacketDeserialized = new ServerStatusPacket(recivedPacket.Data);

            Assert.AreEqual(statusPacketDeserialized.Status, false, "Пакет должен быть статусом не готовности");
            Assert.AreEqual(statusPacketDeserialized.ClientId, (uint) 0, "Номер клиента должен быть 0");
            Assert.AreEqual(statusPacketDeserialized.RegionId, (uint) 0, "Номер региона должен быть 0");
            Assert.AreEqual(statusPacketDeserialized.GlobalId, (uint) 0, "Глобавльный номер должен быть 0");
        }

        [TestMethod]
        public void PacketEndTest()
        {
            var statusPacket = new ServerStatusPacket(true);

            Common.Packet.Packet packet = statusPacket.GetPacket();

            Assert.IsTrue(packet.ToBase64String().Contains(Common.Packet.Packet.PacketEnd));
        }
    }
}
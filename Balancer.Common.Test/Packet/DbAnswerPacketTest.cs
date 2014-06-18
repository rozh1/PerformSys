using System;
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
            dt.TableName = "test table";
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

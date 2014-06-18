using System.Data;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balancer.Common.Test.Packet
{
    [TestClass]
    public class DbAnswerPacketTest
    {
        [TestMethod]
        public void DbAnswerPacketSeralizationTest()
        {
            var dt = new DataTable();
            dt.TableName = "randomTable";
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            var packetBase = new PacketBase
            {
                ClientId = 1,
                GlobalId = 2,
                RegionId = 3
            };

            var dbAnswerPacket = new DbAnswerPacket(dt, 1, packetBase);

            Common.Packet.Packet packet = dbAnswerPacket.GetPacket();

            var dbAnswerPacketDeserialized = new DbAnswerPacket(packet.Data);

            Assert.AreEqual(dbAnswerPacketDeserialized.QueryNumber, dbAnswerPacket.QueryNumber, "Разные номера запросов");
            Assert.AreEqual(dbAnswerPacketDeserialized.ClientId, dbAnswerPacket.ClientId, "Разные номера клиентов");
            Assert.AreEqual(dbAnswerPacketDeserialized.RegionId, dbAnswerPacket.RegionId, "Разные номера регионов");
            Assert.AreEqual(dbAnswerPacketDeserialized.GlobalId, dbAnswerPacket.GlobalId, "Разные глобальные номера");
            Assert.IsTrue(
                CustomComparators.AreTablesEqual(dbAnswerPacketDeserialized.AnswerDataTable,
                    dbAnswerPacket.AnswerDataTable), "Разные данные в ответе");
        }
    }
}
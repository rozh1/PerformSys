﻿using System;
using System.IO;
using System.Net.Sockets;
using Balancer.Common.Packet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace Balancer.Common.Test.Util
{
    [TestClass]
    public class PacketTransmitHelperTest
    {
        [TestMethod]
        public void ReciveSinglePacketTest()
        {
            //var dbRequestPacket = new DbRequestPacket("query", 1);
            //MemoryStream memoryStream = new MemoryStream();
            //byte[] buffer = dbRequestPacket.GetPacket().ToBytes();
            //memoryStream.Write(buffer, 0, buffer.Length);

            //NetworkStream networkStream = new Mock<INetworkStream>//NetworkStream(new Socket(socketType:SocketType.Stream, protocolType:ProtocolType.IP));
            //Moq.AutoMock.AutoMocker networkStreamMock = new AutoMocker();
            //networkStreamMock.CreateInstance<NetworkStream>();
            //networkStreamMock.
            //var networkStreamMock = new Mock<ine>();
            //networkStreamMock.Setup(x => x.DataAvailable).Returns(true);
            //
            //networkStreamMock.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Callback(() =>
            //{
            //    // after the call to Read() re-setup the property so that we
            //    // we exit the data reading loop again
            //    networkStreamMock.Setup(x => x.DataAvailable).Returns(false);
            //
            //}).Returns(buffer.Length);
            //
            //var packet = Balancer.Common.Utils.PacketTransmitHelper.Recive(networkStreamMock.Object);
            //
            //var dbRequestPacketDeserialized = new DbRequestPacket(packet.Data);
            //
            //Assert.AreEqual(dbRequestPacketDeserialized.QueryNumber, dbRequestPacket.QueryNumber, "Разные номера запросов");
            //Assert.AreEqual(dbRequestPacketDeserialized.ClientId, dbRequestPacket.ClientId, "Разные номера клиентов");
            //Assert.AreEqual(dbRequestPacketDeserialized.RegionId, dbRequestPacket.RegionId, "Разные номера регионов");
            //Assert.AreEqual(dbRequestPacketDeserialized.GlobalId, dbRequestPacket.GlobalId, "Разные глобальные номера");
            //Assert.AreEqual(dbRequestPacketDeserialized.Query, dbRequestPacket.Query, "Разные запросы в ответе");
        }
    }
}

using System;
using System.Net.Sockets;
using System.Text;

namespace Balancer.Common.Utils
{
    public class PacketTransmitHelper
    {
        private string _nextPacketData = string.Empty;

        public bool Send(Packet.Packet packet, NetworkStream networkStream)
        {
            bool result = false;
            byte[] packetBytes = packet.ToBytes();
            if (networkStream.CanWrite)
            {
                try
                {
                    networkStream.Write(packetBytes, 0, packetBytes.Length);
                    networkStream.Flush();
                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Write("Исключение при попытке отправки: " + ex.Message);
                }
                return result;
            }
            return false;
        }

        public Packet.Packet Recive(NetworkStream networkStream)
        {
            var buffer = new byte[64000];
            string packetData = "";
            Packet.Packet packet = null;

            if (!string.IsNullOrEmpty(_nextPacketData))
            {
                if (_nextPacketData.Contains(Packet.Packet.PacketEnd))
                {
                    int index = _nextPacketData.IndexOf(Packet.Packet.PacketEnd, StringComparison.Ordinal);
                    packetData =
                        _nextPacketData.Remove(index);
                    _nextPacketData =
                        _nextPacketData.Substring(
                            index +
                            Packet.Packet.PacketEnd.Length);
                    return new Packet.Packet(packetData);
                }
            }

            try
            {
                int count;
                while ((count = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    packetData += _nextPacketData + Encoding.ASCII.GetString(buffer, 0, count);
                    if (packetData.Contains(Packet.Packet.PacketEnd))
                    {
                        int index = packetData.IndexOf(Packet.Packet.PacketEnd, StringComparison.Ordinal);
                        _nextPacketData =
                            packetData.Substring(
                                index +
                                Packet.Packet.PacketEnd.Length);
                        packetData =
                            packetData.Remove(index);
                        break;
                    }
                    _nextPacketData = "";
                }
                if (count > 0)
                {
                    packet = new Packet.Packet(packetData);
                }
                else
                {
                    if (count == 0) Logger.Write("Произошло отключение");
                    if (count < 0) Logger.Write("Ошибка соедиения");
                }
            }
            catch (Exception ex)
            {
                Logger.Write("Исключение при чтении ответа: " + ex.Message);
            }
            return packet;
        }
    }
}
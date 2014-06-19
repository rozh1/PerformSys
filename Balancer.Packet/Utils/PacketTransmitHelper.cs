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
using System.Net.Sockets;
using System.Text;

namespace Balancer.Common.Utils
{
    public static class PacketTransmitHelper
    {
        public static bool Send(Packet.Packet packet, NetworkStream networkStream)
        {
            var result = false;
            var packetBytes = packet.ToBytes();
            if (networkStream.CanWrite)
            {
                try
                {
                    networkStream.Write(packetBytes, 0, packetBytes.Length);
                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Write("Исключение при попытке отправки: " + ex.Message);
                }
                return result;
            }
            return result;
        }

        private static string _nextPacketData = string.Empty;

        public static Packet.Packet Recive(NetworkStream networkStream)
        {
            var buffer = new byte[1400];
            string packetData = "";
            var packet = new Packet.Packet("");
            try
            {
                if (networkStream.DataAvailable)
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
                    packet = new Packet.Packet(packetData);
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

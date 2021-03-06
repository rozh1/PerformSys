﻿#region Copyright
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
using System;
using System.Net.Sockets;
using System.Text;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;

namespace PerformSys.Common.Utils
{
    public class PacketTransmitHelper
    {
        object writeSyncObject = new object();
        object readSyncObject = new object();

        private readonly string _logFilePath;
        public PacketTransmitHelper(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        private string _nextPacketData = string.Empty;

        public bool Send(Packet.Packet packet, NetworkStream networkStream)
        {
            bool result = false;
            byte[] packetBytes = packet.ToBytes();
            if (networkStream.CanWrite)
            {
                try
                {
                    lock (writeSyncObject)
                    {
                        networkStream.Write(packetBytes, 0, packetBytes.Length);
                        networkStream.Flush();
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    Logger.Logger.Write(_logFilePath, 
                        new StringLogData(string.Format(
                            @"Исключение при попытке отправки: {0} ", ex.Message
                            )), LogLevel.ERROR);
                }
                return result;
            }
            return false;
        }

        public Packet.Packet Recive(NetworkStream networkStream)
        {
            var buffer = new byte[1024*1024*2];
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
                    if (count == 0)
                        Logger.Logger.Write(_logFilePath, new StringLogData(
                            @"Произошло отключение"), LogLevel.ERROR);
                    if (count < 0)
                        Logger.Logger.Write(_logFilePath, new StringLogData(
                            @"Ошибка приема"), LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Write(_logFilePath,
                    new StringLogData(string.Format(
                        @"Исключение при чтении ответа: {0} ", ex.Message
                        )), LogLevel.ERROR);
            }
            return packet;
        }
    }
}
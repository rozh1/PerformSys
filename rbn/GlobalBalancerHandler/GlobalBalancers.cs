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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using rbn.QueueHandler;

namespace rbn.GlobalBalancerHandler
{
    internal class GlobalBalancers
    {
        private static List<GlobalBalancer> _globalBalancers;
        private static List<Thread> _globalBalancersThreads;
        private static TcpListener _listener;
        private static bool _serverIsLife;

        public static void Init()
        {
            _globalBalancers = new List<GlobalBalancer>();
            _globalBalancersThreads = new List<Thread>();

            int port = 3400;
            int.TryParse(ConfigFile.GetConfigValue("Server_count"), out port);

            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            _serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                var globalBalancer = new GlobalBalancer
                {
                    Connection = tcpClient,
                    Status = false,
                    StatusRecived = false
                };
                AddGlobalBalancer(globalBalancer);
                var t = new Thread(GlobalBalancerListenThread);
                _globalBalancersThreads.Add(t);
                t.Start(globalBalancer);
            }
        }

        public static void AddGlobalBalancer(GlobalBalancer server)
        {
            if (!_globalBalancers.Contains(server))
            {
                _globalBalancers.Add(server);
            }
        }

        public static void RemoveGlobalBalancer(GlobalBalancer server)
        {
            if (_globalBalancers.Contains(server))
            {
                _globalBalancers.Remove(server);
            }
        }

        private static void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (GlobalBalancer) param;
            TcpClient connection = globalBalancer.Connection;
            string nextPacketData = "";
            while (connection.Connected)
            {
                string packetData = "";
                var buffer = new byte[1400];

                try
                {
                    int count;
                    while ((count = connection.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    {
                        packetData += nextPacketData + Encoding.ASCII.GetString(buffer, 0, count);
                        if (packetData.Contains(Packet.PacketEnd))
                        {
                            int index = packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal);
                            nextPacketData =
                                packetData.Substring(
                                    index +
                                    Packet.PacketEnd.Length);
                            packetData =
                                packetData.Remove(index);
                            break;
                        }
                        nextPacketData = "";
                    }
                    var packet = new Packet(packetData);

                    switch (packet.Type)
                    {
                        case PacketType.Status:
                            var sp = new StatusPacket(packet.Data);
                            globalBalancer.Status = sp.Status;
                            globalBalancer.StatusRecived = true;
                            if (globalBalancer.Status) RbnQueue.SendRequestToServer();
                            break;
                        case PacketType.Answer:
                            RbnQueue.ServerAnswer(int.Parse(packet.ClientId), packet.Data);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write("Исключение при чтении ответа: " + ex.Message);
                }
            }
        }
    }
}
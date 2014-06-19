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
        private static GlobalBalancer _globalBalancer;
        private static Thread _globalBalancersThread;
        private static TcpListener _listener;
        private static bool _serverIsLife;

        public static void Init()
        {
            int port = (int)Config.RBNConfig.Instance.RBN.GlobalBalancerPort;

            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            _serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                _globalBalancer = new GlobalBalancer
                {
                    Connection = tcpClient,
                    Status = false,
                    StatusRecived = false
                };
                _globalBalancersThread = new Thread(GlobalBalancerListenThread);
                _globalBalancersThread.Start(_globalBalancer);
            }
        }

        private static void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (GlobalBalancer) param;
            TcpClient connection = globalBalancer.Connection;

            while (connection.Connected)
            {
                var packet = Balancer.Common.Utils.PacketTransmitHelper.Recive(connection.GetStream());

                switch (packet.Type)
                {
                    case PacketType.Status:
                        var sp = new StatusPacket(packet.Data);
                        globalBalancer.Status = sp.Status;
                        globalBalancer.StatusRecived = true;
                        if (globalBalancer.Status) RbnQueue.SendRequestToServer();
                        break;
                    case PacketType.Answer:
                        var answer = new DbAnswerPacket(packet.Data);
                        RbnQueue.ServerAnswer((int) answer.ClientId, packet.Data);
                        break;
                }
            }
        }
    }
}
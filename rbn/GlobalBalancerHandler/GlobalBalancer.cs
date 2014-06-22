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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Config;
using rbn.Interfaces;
using rbn.QueueHandler;

namespace rbn.GlobalBalancerHandler
{
    internal class GlobalBalancer : IServer
    {
        private Data.GlobalBalancer _globalBalancer;
        private Thread _globalBalancersThread;
        private TcpListener _listener;

        public event Action<IServer> SendRequestFromQueueEvent;

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        public bool SendRequest(QueueEntity queueEntity)
        {
            Data.GlobalBalancer globalBalancer = _globalBalancer;
            if (globalBalancer != null)
            {
                if (!globalBalancer.Connection.Connected) return false;
                var dbRequestPacket = new DbRequestPacket(queueEntity.RequestData);
                if (!PacketTransmitHelper.Send(dbRequestPacket.GetPacket(), globalBalancer.Connection.GetStream()))
                    return false;
                Logger.Write("Отправлен запрос от клиента " + queueEntity.ClientId);
            }
            else return false;
            return true;
        }

        /// <summary>
        ///     Событие получения ответа
        /// </summary>
        public event Action<int, DbAnswerPacket> AnswerRecivedEvent;

        private void Init()
        {
            var port = (int) RBNConfig.Instance.RBN.GlobalBalancerPort;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            TcpClient tcpClient = _listener.AcceptTcpClient();
            Logger.Write("Принято соединие");

            _globalBalancer = new Data.GlobalBalancer
            {
                Connection = tcpClient,
                Status = false,
                StatusRecived = false
            };
            _globalBalancersThread = new Thread(GlobalBalancerListenThread);
            _globalBalancersThread.Start(_globalBalancer);
        }

        private void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer) param;
            TcpClient connection = globalBalancer.Connection;

            while (connection.Connected)
            {
                Packet packet = PacketTransmitHelper.Recive(connection.GetStream());

                switch (packet.Type)
                {
                    case PacketType.TransmitRequest:
                        if (SendRequestFromQueueEvent != null) SendRequestFromQueueEvent(this);
                        break;
                    case PacketType.Answer:
                        var answer = new DbAnswerPacket(packet.Data);
                        if (AnswerRecivedEvent != null)
                            AnswerRecivedEvent((int) answer.ClientId, new DbAnswerPacket(packet.Data));
                        break;
                }
            }
            Init();
        }
    }
}
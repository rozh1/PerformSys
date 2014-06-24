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
    internal class GlobalBalancer : IServer, IDisposable
    {
        private readonly Data.GlobalBalancer _globalBalancer;

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

        public GlobalBalancer()
        {
            _globalBalancer = new Data.GlobalBalancer
            {
                Connection = null,
                Status = true,
                StatusRecived = false
            };
            var globalBalancersThread = new Thread(GlobalBalancerListenThread);
            globalBalancersThread.Start(_globalBalancer);
        }

        private void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer) param;
            TcpClient connection = globalBalancer.Connection;

            while (globalBalancer.Status)
            {
                try
                {
                    connection.Connect(
                        RBNConfig.Instance.MRBN.Host,
                        (int)RBNConfig.Instance.MRBN.Port);
                }
                catch (Exception)
                {
                    Logger.Write("Не удалось подключиться к межрегиональному балансировщику. Переподключение");
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Write("Подключение установлено");

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
            }
        }

        public void Dispose()
        {
            _globalBalancer.Status = false;
            _globalBalancer.Connection.Close();
        }

        ~GlobalBalancer()
        {
            Dispose();
        }
    }
}
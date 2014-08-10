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

﻿using System.Net;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Logger;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.GlobalBalancerHandler;
using rbn.QueueHandler;
using rbn.QueueHandler.Data;
using rbn.ServersHandler;

namespace rbn
{
    /// <summary>
    ///     РБН
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Слушатель новых соединений (клиентов)
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        ///     признак жизн потока
        /// </summary>
        private bool _serverIsLife;

        /// <summary>
        /// Очередь регионального балансировщика
        /// </summary>
        private readonly RbnQueue _rbnQueue;

        public Server(int port)
        {
            _rbnQueue = new RbnQueue();
            var servers = new Servers((int)Config.RBNConfig.Instance.Server.Port);
            var globalBalancer = new GlobalBalancer();
            
            servers.AnswerRecivedEvent += _rbnQueue.ServerAnswer;
            servers.SendRequestFromQueueEvent += _rbnQueue.SendRequestToServer;
            servers.DataBaseInfoRecivedEvent += _rbnQueue.AddDataBaseInfo;

            globalBalancer.RequestRecivedEvent += _rbnQueue.AddClient;
            globalBalancer.AnswerRecivedEvent += _rbnQueue.ServerAnswer;
            globalBalancer.SendRequestFromQueueEvent += _rbnQueue.SendRequestToServer;
            globalBalancer.QueryWeightComputeEvent += _rbnQueue.ComputeQueueWeight;

            _serverIsLife = true;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write("Начато прослушивание клиентов " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                var t = new Thread(ClientThread);
                t.Start(tcpClient);
            }
        }

        /// <summary>
        ///     Поток обслуживания клиента
        /// </summary>
        /// <param name="param"></param>
        private void ClientThread(object param)
        {
            var transmitHelper = new PacketTransmitHelper();
            var tcpClient = (TcpClient) param;
            var client = new Client();
            while (tcpClient.Connected)
            {
                var packet = transmitHelper.Recive(tcpClient.GetStream());
                if (packet != null)
                {
                    var dbRequestPacket = new DbRequestPacket(packet.Data)
                    {
                        GlobalId = Config.RBNConfig.Instance.RBN.GlobalId,
                        RegionId = Config.RBNConfig.Instance.RBN.RegionId
                    };

                    client.Connection = tcpClient;
                    client.RequestPacketData = dbRequestPacket.GetPacket().Data;

                    _rbnQueue.AddClient(client);
                }
                else
                {
                    tcpClient.Close();
                }
            }
            Logger.Write("Клиент отключен:");
            _rbnQueue.RemoveClient(client);
        }

        /// <summary>
        ///     убийца - деструктор
        /// </summary>
        ~Server()
        {
            _serverIsLife = false;
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
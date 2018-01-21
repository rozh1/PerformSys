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
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Packet.Packets;
using PerformSys.Common.Utils;
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
        /// Очередь регионального балансировщика
        /// </summary>
        private readonly RbnQueue _rbnQueue;

        public Server(int port)
        {
            _rbnQueue = new RbnQueue();
            var servers = new Servers((int)Config.RBNConfig.Instance.Server.Port);
            
            servers.AnswerRecivedEvent += _rbnQueue.ServerAnswer;
            servers.SendRequestFromQueueEvent += _rbnQueue.SendRequestToServer;
            servers.DataBaseInfoRecivedEvent += _rbnQueue.AddDataBaseInfo;

            if (Config.RBNConfig.Instance.MRBN.UseMRBN)
            {
                var globalBalancer = new GlobalBalancer();
                globalBalancer.RequestRecivedEvent += _rbnQueue.AddClient;
                globalBalancer.AnswerRecivedEvent += _rbnQueue.ServerAnswer;
                globalBalancer.SendRequestFromQueueEvent += _rbnQueue.SendRequestToAnotherRegion;
                globalBalancer.QueryWeightComputeEvent += _rbnQueue.ComputeQueueWeight;
            }

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write(Config.RBNConfig.Instance.Log.LogFile, 
                new StringLogData("Начато прослушивание клиентов " + IPAddress.Any + ":" + port), 
                LogLevel.INFO);

            _listener.BeginAcceptTcpClient(OnAcceptConnection, _listener);

        }

        private void OnAcceptConnection(IAsyncResult asyn)
        {
            var listener = (TcpListener)asyn.AsyncState;
            TcpClient tcpClient = listener.EndAcceptTcpClient(asyn);
            
            Logger.Write(Config.RBNConfig.Instance.Log.LogFile,
                new StringLogData("Принято соединие"),
                LogLevel.INFO);
            
            var t = new Thread(ClientThread);
            t.Start(tcpClient);

            listener.BeginAcceptTcpClient(OnAcceptConnection, listener);
        }

        /// <summary>
        ///     Поток обслуживания клиента
        /// </summary>
        /// <param name="param"></param>
        private void ClientThread(object param)
        {
            var transmitHelper = new PacketTransmitHelper(Config.RBNConfig.Instance.Log.LogFile);
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
            Logger.Write(Config.RBNConfig.Instance.Log.LogFile, new StringLogData("Клиент отключен"), LogLevel.INFO);
            _rbnQueue.RemoveClient(client);
        }

        /// <summary>
        ///     убийца - деструктор
        /// </summary>
        ~Server()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
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

﻿using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using mrbn.Config.Data.LogData;
using mrbn.GlobalBalancer.Data;

namespace mrbn
{
    /// <summary>
    ///     МРБН
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Балансировщик между регионами
        /// </summary>
        private readonly GlobalBalancer.GlobalBalancer _balancer;

        /// <summary>
        ///     Слушатель новых соединений (клиентов)
        /// </summary>
        private readonly TcpListener _listener;
        
        /// <summary>
        ///     признак жизн потока
        /// </summary>
        private bool _serverIsLife;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write(Config.MRBNConfig.Instance.Log.LogFile, new StringLogData("Начато прослушивание " + IPAddress.Any + ":" + port), LogLevel.INFO);

            _balancer = new GlobalBalancer.GlobalBalancer();

            _serverIsLife = true;

            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write(Config.MRBNConfig.Instance.Log.LogFile, new StringLogData("Принято соединие"), LogLevel.INFO);

                var t = new Thread(ClientThread);
                t.Start(tcpClient);
            }
        }

        /// <summary>
        ///     Поток обслуживания клиентского соедиения
        /// </summary>
        /// <param name="param"></param>
        private void ClientThread(object param)
        {
            var transmitHelper = new PacketTransmitHelper(Config.MRBNConfig.Instance.Log.LogFile);
            var rbnClient = (TcpClient) param;

            Packet statusPacket = transmitHelper.Recive(rbnClient.GetStream());
            if (statusPacket == null) return;
            if (statusPacket.Type != PacketType.RBNStatus) return;

            var rbnStatusPacket = new RBNStatusPacket(statusPacket.Data);
            var rbn = new RBN
            {
                RbnClient = rbnClient,
                RegionId = rbnStatusPacket.RegionId,
                GlobalId = rbnStatusPacket.GlobalId,
                Weight = 0
            };

            if (!_balancer.AddRbn(rbn))
            {
                rbnClient.Close();
                return;
            }
            
            while (rbnClient.Connected)
            {
                Packet packet = transmitHelper.Recive(rbnClient.GetStream());
                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.RBNStatus:
                            rbn.Weight = (new RBNStatusPacket(packet.Data)).Weight;
                            //Logger.Write(string.Format("Получен вес очереди {0} РБН {1}", rbn.RegionId, rbn.Weight));
                            break;
                        case PacketType.Request:
                            Debug.Assert(rbn.RelayRbn.RbnClient != null, "rbn.RelayRbn.RbnClient != null");
                            var dbRequestPacket = new DbRequestPacket(packet.Data);
                            
                            Logger.Write(Config.MRBNConfig.Instance.Log.LogFile, new StringLogData(string.Format("Предача запроса из {0} в {1} РБН", rbn.RegionId, rbn.RelayRbn.RegionId)), LogLevel.INFO);
                            Logger.Write(Config.MRBNConfig.Instance.Log.StatsFile,
                                new Transmit(
                                    dbRequestPacket.GlobalId,
                                    dbRequestPacket.RegionId,
                                    (int) dbRequestPacket.ClientId,
                                    dbRequestPacket.QueryNumber,
                                    rbn.Weight,
                                    rbn.RelayRbn.GlobalId,
                                    rbn.RelayRbn.RegionId,
                                    rbn.RelayRbn.Weight
                                    ),
                                LogLevel.INFO
                                );

                            if (transmitHelper.Send(packet, rbn.RelayRbn.RbnClient.GetStream()))
                            {
                                rbn.RelayRbn = null;
                            }
                            break;
                        case PacketType.Answer: 
                            var dbAnswerPacket = new DbAnswerPacket(packet.Data);
                            RBN remoteRbn = _balancer.GetRbnByRegionId((int) dbAnswerPacket.RegionId);
                            Logger.Write(Config.MRBNConfig.Instance.Log.LogFile, new StringLogData(string.Format("Получен ответ для {1} из {0} РБН", rbn.RegionId, remoteRbn.RegionId)), LogLevel.INFO);
                            Debug.Assert(rbn.RegionId != remoteRbn.RegionId, "rbn.RegionId == remoteRbn.RegionId");
                            transmitHelper.Send(packet, remoteRbn.RbnClient.GetStream());
                            break;
                    }
                }
                _balancer.ConnectRbns();
                if (rbn.RelayRbn != null && rbn.RelayRbn.RbnClient != null)
                {
                    transmitHelper.Send((new TransmitRequestPacket()).GetPacket(), rbn.RbnClient.GetStream());
                }
            }

            _balancer.Remove(rbn);
            rbnClient.Close();
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
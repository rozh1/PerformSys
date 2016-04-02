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
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using router.Config;
using router.Config.Data.LogData;
using router.ServersHandler;

namespace router
{
    /// <summary>
    ///     Роутер
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     текущее содинение
        /// </summary>
        private readonly TcpClient _tcpClient;
        

        /// <summary>
        ///     длина очереди
        /// </summary>
        private int _queueLength;

        /// <summary>
        ///     признак жизни потока слушателя
        /// </summary>
        private bool _serverIsLife = true;

        private PacketTransmitHelper _transmitHelper;


        /// <summary>
        /// Очередь регионального балансировщика
        /// </summary>
        private readonly RouterQueue _routerQueue;
        private readonly Servers _routerServers;

        public Server()
        {
            _routerQueue = new RouterQueue();
            _routerServers = new Servers(RouterConfig.Instance.Router.Port);

            _transmitHelper = new PacketTransmitHelper(RouterConfig.Instance.Log.LogFile);
            _tcpClient = new TcpClient();
            while (_serverIsLife)
            {
                try
                {
                    _tcpClient.Connect(
                        RouterConfig.Instance.Router.RBN.Host, 
                        (int)RouterConfig.Instance.Router.RBN.Port);
                }
                catch (Exception)
                {
                    Logger.Write(RouterConfig.Instance.Log.LogFile, 
                        new StringLogData("Не удалось подключиться. Переподключение"), 
                        LogLevel.ERROR);
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Write(RouterConfig.Instance.Log.LogFile, 
                    new StringLogData("Подключение установлено"), 
                    LogLevel.INFO);

                _routerServers.SendRequestFromQueueEvent += _routerQueue.SendRequestToServer;
                _routerServers.PacketRecieved += RouterServersOnPacketRecieved;

                while (_tcpClient.Connected)
                {
                    SendStatus();
                    Packet onePacketData = null;
                    try
                    {
                        onePacketData = _transmitHelper.Recive(_tcpClient.GetStream());
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(RouterConfig.Instance.Log.LogFile,
                            new StringErorrLogData(
                                "{0}. Ошибка получения пакета: {1}",
                                System.Reflection.MethodBase.GetCurrentMethod().Name,
                                ex.Message),
                            LogLevel.ERROR);
                    } 

                    if (onePacketData != null)
                    {
                        _queueLength++;

                        PacketBase packetBase = new PacketBase();
                        packetBase.Deserialize(onePacketData.Data);
                        Logger.Write(RouterConfig.Instance.Log.QueueStatsFile,
                            new QueueStats(packetBase.GlobalId, packetBase.RegionId, (int) packetBase.ClientId,
                                _queueLength),
                            LogLevel.INFO);

                        //SendStatus();
                        AddQueryToQueue(onePacketData);
                    }
                    else
                    {
                        Logger.Write(RouterConfig.Instance.Log.LogFile, 
                            new StringLogData("Получен пустой пакет или разорвано соединение"), 
                            LogLevel.ERROR);
                    }
                }

                _routerServers.SendRequestFromQueueEvent -= _routerQueue.SendRequestToServer;
                _routerServers.PacketRecieved -= RouterServersOnPacketRecieved;

                _tcpClient.Close();
                _tcpClient = new TcpClient();
            }
        }

        private void RouterServersOnPacketRecieved(Packet packet)
        {
            if (packet.Type == PacketType.Answer)
            {
                _queueLength--;
                Logger.Write(RouterConfig.Instance.Log.LogFile,
                    new StringLogData("Длина очереди " + _queueLength),
                    LogLevel.INFO);
            }

            _transmitHelper.Send(packet, _tcpClient.GetStream());
            SendStatus();
        }

        private void AddQueryToQueue(Packet packet)
        {
            var requestPacket = new DbRequestPacket(packet.Data);
            _routerQueue.AddQuery(requestPacket);
        }
        
        /// <summary>
        ///     Отправка статуса сервера
        /// </summary>
        private void SendStatus()
        {
            var canBeSended = _routerServers.GetServersCount()*9;
            bool status = (_queueLength - canBeSended < 3);
            var sp = new ServerStatusPacket(status);
            if (_tcpClient.Connected)
            {
                _transmitHelper.Send(sp.GetPacket(), _tcpClient.GetStream());
                Logger.Write(RouterConfig.Instance.Log.LogFile, 
                    new StringLogData("Отослан статус " + status), 
                    LogLevel.INFO);
            }
        }
        
        /// <summary>
        ///     убийца - деструктор
        /// </summary>
        ~Server()
        {
            _serverIsLife = false;
            if (_tcpClient != null)
            {
                if ( _tcpClient.Connected) _tcpClient.Close();
            }
        }
    }
}
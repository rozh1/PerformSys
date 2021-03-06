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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Packet;
using PerformSys.Common.Packet.Packets;
using PerformSys.Common.Utils;
using rbn.Interfaces;
using rbn.QueueHandler.Data;

namespace rbn.ServersHandler
{
    /// <summary>
    ///     Сервера БД
    /// </summary>
    public class Servers : IServer, IDisposable
    {
        /// <summary>
        ///     Список серверов
        /// </summary>
        private readonly List<Server> _serversList;

        /// <summary>
        ///     Список потоков серверов
        /// </summary>
        private readonly List<Thread> _serversThreads;

        /// <summary>
        ///     Индекс сервера, на каторый был оправлен последний запрос
        /// </summary>
        private int _lastReadyServerIndex;

        /// <summary>
        ///     Признак работы потока отправки
        /// </summary>
        private bool _sendThreadLife = true;

        /// <summary>
        ///     Признак работы потока приема соединений от серверов
        /// </summary>
        private bool _serverIsLife = true;

        /// <summary>
        ///     Слушатель соедиенений
        /// </summary>
        private readonly TcpListener _listener;

        private readonly PacketTransmitHelper _transmitHelper;

        public Servers(int port)
        {
            _transmitHelper = new PacketTransmitHelper(Config.RBNConfig.Instance.Log.LogFile);
            _serversList = new List<Server>();
            _serversThreads = new List<Thread>();

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write(Config.RBNConfig.Instance.Log.LogFile, 
                new StringLogData("Начато прослушивание серверов " + IPAddress.Any + ":" + port), 
                LogLevel.INFO);

            var thread = new Thread(SendThread);
            thread.Start();

            thread = new Thread(ServerThread);
            thread.Start();
        }

        /// <summary>
        ///     Событие иницирования отправки запроса из очереди
        /// </summary>
        public event Action<IServer> SendRequestFromQueueEvent;

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        public bool SendRequest(QueueEntity queueEntity)
        {
            Server server = GetNextReadyServer();
            if (server != null)
            {
                SendRequest(server, queueEntity.RequestPacket);
                Logger.Write(Config.RBNConfig.Instance.Log.LogFile,
                    new StringLogData("Отправлен запрос от клиента " + queueEntity.ClientId), 
                    LogLevel.INFO);
            }
            else return false;
            return true;
        }

        /// <summary>
        ///     Событие получения ответа
        /// </summary>
        public event Action<DbAnswerPacket> AnswerRecivedEvent;

        /// <summary>
        ///     Событие информации о БД
        /// </summary>
        public event Action<DataBaseInfoPacket> DataBaseInfoRecivedEvent;

        /// <summary>
        ///     Поток отправки запросов серверам
        /// </summary>
        private void SendThread()
        {
            while (_sendThreadLife)
            {
                if (SendRequestFromQueueEvent != null) SendRequestFromQueueEvent(this);
                Thread.Sleep(30);
            }
        }

        /// <summary>
        ///     Поток отправки запросов серверам
        /// </summary>
        private void ServerThread()
        {
            while (_sendThreadLife)
            {
                while (_serverIsLife)
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    Logger.Write(Config.RBNConfig.Instance.Log.LogFile,new StringLogData("Подключен сервер"), LogLevel.INFO);
                    var server = new Server { Connection = tcpClient };
                    AddServer(server);
                    var serverThread = new Thread(ServerListenThread);
                    _serversThreads.Add(serverThread);
                    serverThread.Start(server);
                }
            }
        }
        /// <summary>
        ///     Добавление сервера в список
        /// </summary>
        /// <param name="server"></param>
        private void AddServer(Server server)
        {
            if (!_serversList.Contains(server))
            {
                _serversList.Add(server);
            }
        }

        /// <summary>
        ///     Удаление сервера из списка
        /// </summary>
        /// <param name="server"></param>
        private void RemoveServer(Server server)
        {
            if (_serversList.Contains(server))
            {
                server.Connection.Close();
                _serversList.Remove(server);
            }
        }

        /// <summary>
        ///     Получение следующего свободного сервера по круговому алгоритму
        /// </summary>
        /// <returns></returns>
        private Server GetNextReadyServer()
        {
            for (int i = _lastReadyServerIndex; i < _serversList.Count; i++)
            {
                if (_serversList[i].Status && _serversList[i].StatusRecived)
                {
                    _lastReadyServerIndex = i + 1;
                    return _serversList[i];
                }
            }
            for (int i = 0; i < _serversList.Count; i++)
            {
                if (_serversList[i].Status && _serversList[i].StatusRecived)
                {
                    _lastReadyServerIndex = i;
                    return _serversList[i];
                }
            }
            return null;
        }

        /// <summary>
        ///     Поток обработки ответов сервера
        /// </summary>
        /// <param name="param"></param>
        private void ServerListenThread(object param)
        {
            var server = (Server) param;
            TcpClient connection = server.Connection;
            while (connection.Connected)
            {
                Packet packet = _transmitHelper.Recive(connection.GetStream());

                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.ServerStatus:
                            var sp = new ServerStatusPacket(packet.Data);
                            server.Status = sp.Status;
                            server.StatusRecived = true;
                            break;
                        case PacketType.Answer:
                            var answer = new DbAnswerPacket(packet.Data);
                            if (AnswerRecivedEvent != null)
                                AnswerRecivedEvent.BeginInvoke(answer,null,null);
                            break;
                        case PacketType.DataBaseInfo:
                            var dataBaseInfoPacket = new DataBaseInfoPacket(packet.Data)
                            {
                                GlobalId = Config.RBNConfig.Instance.RBN.GlobalId
                            };
                            if (DataBaseInfoRecivedEvent != null) DataBaseInfoRecivedEvent(dataBaseInfoPacket);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Servers. Получен пакет - " + packet.Type);
                    }
                }
                else
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dbRequestPacket"></param>
        private void SendRequest(Server server, DbRequestPacket dbRequestPacket)
        {
            if (!server.Connection.Connected) return;
            if (_transmitHelper.Send(dbRequestPacket.GetPacket(), server.Connection.GetStream()))
                server.StatusRecived = false;
        }

        /// <summary>
        ///     убийца
        /// </summary>
        ~Servers()
        {
            Dispose();
        }

        /// <summary>
        /// Приказано уничтожить
        /// </summary>
        public void Dispose()
        {
            _serverIsLife = false;
            _sendThreadLife = false;
            foreach (Server server in _serversList)
            {
                RemoveServer(server);
            }
        }
    }
}
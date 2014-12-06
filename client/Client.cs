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
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using client.Config.Data;
using client.Properties;

namespace client
{
    /// <summary>
    ///     Эмулятор клиента.
    /// </summary>
    internal class Client
    {
        private readonly string _address;
        private readonly int _clientId;
        private readonly Config.ClientConfig _config;
        private readonly PacketTransmitHelper _packetTransmitHelper;
        private readonly int _port;
        private readonly QuerySequence.IQuerySequence _querySequence;
        private readonly ScenarioStep[] _scenarioSteps;
        private int _queryNumber;
        private DateTime _starTime;
        private TcpClient _tcpClient;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="config">Конфигурация клиента.</param>
        /// <param name="clientId">Идентификатор клиента.</param>
        /// <param name="querySequence">Последовательность запросов.</param>
        public Client(Config.ClientConfig config,
            int clientId,
            QuerySequence.IQuerySequence querySequence)
        {
            _address = config.Server.Host;
            Debug.Assert(config.Server.Host != null, "config.BalancerPort != null");
            _port = (int) config.Server.Port;
            _clientId = clientId;
            _scenarioSteps = config.Scenario.ScenarioSteps;
            _querySequence = querySequence;
            _config = config;
            _packetTransmitHelper = new PacketTransmitHelper(_config.Log.LogFile);
            _starTime = config.Scenario.StartTime;

            Connect();

            var t = new Thread(ClientThread);
            t.Start();
        }

        private TcpClient Connect()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
            }

            if (!_tcpClient.Connected)
            {
                try
                {
                    _tcpClient.Connect(_address, _port);
                }
                catch (SocketException se)
                {
                    Logger.Write(_config.Log.LogFile,
                        new StringErorrLogData("{0}. Ошибка соединения {2}. {1}", "Connect",
                            _clientId.ToString(CultureInfo.InvariantCulture),
                            se.SocketErrorCode.ToString()),
                        LogLevel.INFO);
                    return new TcpClient();
                }
            }

            return _tcpClient;
        }

        /// <summary>
        ///     поток эмулятора клиента
        /// </summary>
        private void ClientThread()
        {
            int loopNumber = 0;
            var tcpClient = Connect();
            var clientStatsData = new ClientStatsData();

            while (DateTime.Now < _starTime)
            {
                Thread.Sleep(100);
            }
            
            var scenarioStepsManager = new ScenarioStepsManager(_scenarioSteps);

            while (true)
            {
                ScenarioActions scenarioAction = scenarioStepsManager.GetCurrentScenarioAction();

                if (scenarioAction == ScenarioActions.Sleep)
                {
                    Thread.Sleep(100);
                    continue;
                }
                if (scenarioAction == ScenarioActions.Stop && _config.QuerySequence.Mode != QuerySequenceMode.FromList)
                {
                    break;
                }
                if (!_querySequence.CanGetNextQueryNumber())
                {
                    break;
                }

                if (tcpClient.Connected)
                {
                    loopNumber++;

                    _queryNumber = _querySequence.GetNextQueryNumber();
                    string query = Resources.ResourceManager.GetString("q" + _queryNumber);

                    var dbRequestPacket = new DbRequestPacket(query, _queryNumber)
                    {
                        ClientId = (uint) _clientId
                    };

                    _packetTransmitHelper.Send(dbRequestPacket.GetPacket(), tcpClient.GetStream());

                    DateTime startTime = DateTime.UtcNow;

                    Packet packet = _packetTransmitHelper.Recive(tcpClient.GetStream());

                    //var dt = (DataTable)SerializeMapper.Deserialize(packet.Data);
                    //
                    //string answer = "";
                    //for (int i = 0; i < dt.Columns.Count; i++)  answer += dt.Columns[i].ColumnName + "\t";
                    //for (int j = 0; j < dt.Rows.Count; j++)
                    //{
                    //    answer += "\n";
                    //    for (int i = 0; i < dt.Columns.Count; i++) answer += dt.Rows[j][i] + "\t";
                    //}

                    TimeSpan queryTime = DateTime.UtcNow - startTime;
                    clientStatsData.WaitTime += queryTime;
                    clientStatsData.Answer = null; //answer;

                    Logger.Write(_config.Log.LogFile,
                        new StringLogData(string.Format(@"Клиент: {0}	Запрос: {1}	Время выполнения: {2}", _clientId,
                            loopNumber, queryTime)),
                        LogLevel.INFO);

                    var logEntity = new LogStats()
                    {
                        ClientNumber = _clientId,
                        ClientQueryNumber = loopNumber,
                        QueryNumber = _queryNumber,
                        QueryTime = queryTime
                    };

                    Logger.Write(_config.Log.StatsFile, logEntity, LogLevel.INFO);
                }
                else
                {
                    Logger.Write(_config.Log.LogFile,
                        new StringErorrLogData("{0}. Соедиение потеряно для {1}. Переподключение", "ClientThread",
                            _clientId.ToString(CultureInfo.InvariantCulture)),
                        LogLevel.INFO);
                    tcpClient = Connect();
                }
            }
            tcpClient.Close();

            Logger.Write(_config.Log.LogFile,
                new StringLogData(string.Format(@"Клиент: {0}	Общее время работы: {1}", _clientId,
                    clientStatsData.WaitTime)),
                LogLevel.INFO);
        }
    }
}
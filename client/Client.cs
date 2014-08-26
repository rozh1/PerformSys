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
using System.Net.Sockets;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using client.Properties;
using client.QuerySequence;

namespace client
{
    /// <summary>
    /// Эмулятор клиента.
    /// </summary>
    internal class Client
    {
        private readonly string _address;
        private readonly Config.Config _config;
        private readonly int _clientId;
        private readonly int _port;
        private int _queryNumber;
        private readonly PacketTransmitHelper _packetTransmitHelper;
        private ClientStatsData _clientStatsData;
        private readonly QuerySequence.QuerySequence _querySequence;
        private int _querySeqIndex;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="config">Конфигурация клиента.</param>
        /// <param name="clientId">Идентификатор клиента.</param>
        /// <param name="querySequence">Последовательность запросов.</param>
        public Client(Config.Config config, int clientId, QuerySequence.QuerySequence querySequence)
        {
            _address = config.BalancerHost;
            Debug.Assert(config.BalancerPort != null, "config.BalancerPort != null");
            _port = (int) config.BalancerPort;
            _clientId = clientId;

            _querySequence = querySequence;

            _config = config;
            _packetTransmitHelper = new PacketTransmitHelper("clentLog.txt");
            var t = new Thread(ClientThread);
            t.Start();
        }

        /// <summary>
        ///     поток эмулятора клиента
        /// </summary>
        private void ClientThread()
        {
            _clientStatsData = new ClientStatsData();

            var tcpClient = new TcpClient();
            tcpClient.Connect(_address, _port);
            for (int i = 0; i < _config.QueryCount; i++)
            {
                if (tcpClient.Connected)
                {                  
                    _queryNumber = _querySequence.GetNextQueryNumber();
                    string query = Resources.ResourceManager.GetString("q" + _queryNumber);

                    var dbRequestPacket = new DbRequestPacket(query, _queryNumber)
                    {
                        ClientId = (uint)_clientId
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
                    _clientStatsData.WaitTime += queryTime;
                    _clientStatsData.Answer = null; //answer;
                    Console.WriteLine(@"Клиент: {0}	Запрос: {1}	Время выполнения: {2}", _clientId, i, queryTime);

                    _config.LogStats.ClientNumber = _clientId;
                    _config.LogStats.ClientQueryNumber = i;
                    _config.LogStats.QueryNumber = _queryNumber;
                    _config.LogStats.QueryTime = queryTime;
                    
                    Logger.Write("clientStats.csv", _config.LogStats, LogLevel.INFO);
                }
            }
            tcpClient.Close();
            Console.WriteLine(@"Клиент: {0}	Общее время работы: {1}", _clientId, _clientStatsData.WaitTime);
        }
    }
}
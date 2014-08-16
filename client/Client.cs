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
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Logger;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using client.Properties;

namespace client
{
    /// <summary>
    ///     эмулятор клиента
    /// </summary>
    internal class Client
    {
        private readonly string _address;
        private readonly int _number;
        private readonly int _port;
        private readonly int _queryNumber;
        private ClientStatsData _clientStatsData;
        private readonly Config.Config _config;

        public Client(Config.Config config, int number, int queryNumber)
        {
            _address = config.BalancerHost;
            Debug.Assert(config.BalancerPort != null, "config.BalancerPort != null");
            _port = (int)config.BalancerPort;
            _number = number;
            _queryNumber = queryNumber;
            _config = config;
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
                    string query = Resources.ResourceManager.GetString("q" + _queryNumber);
                    var dbRequestPacket = new DbRequestPacket(query, _queryNumber);
                    Byte[] requestPacket = dbRequestPacket.GetPacket().ToBytes();
                    tcpClient.GetStream().Write(requestPacket, 0, requestPacket.Length);

                    DateTime startTime = DateTime.UtcNow;

                    var buffer = new byte[1400];
                    Packet packet;
                    do
                    {
                        string packetData = "";

                        int count;
                        while ((count = tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                        {
                            packetData += Encoding.ASCII.GetString(buffer, 0, count);
                            if (packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal) >= 0) break;
                        }
                        packet = new Packet(packetData);
                    } while (packet.Type != PacketType.Answer);

                    //var dt = (DataTable)SerializeMapper.Deserialize(packet.Data);
                    //
                    //string answer = "";
                    //for (int i = 0; i < dt.Columns.Count; i++)  answer += dt.Columns[i].ColumnName + "\t";
                    //for (int j = 0; j < dt.Rows.Count; j++)
                    //{
                    //    answer += "\n";
                    //    for (int i = 0; i < dt.Columns.Count; i++) answer += dt.Rows[j][i] + "\t";
                    //}

                    var queryTime = DateTime.UtcNow - startTime;
                    _clientStatsData.WaitTime += queryTime;
                    _clientStatsData.Answer = null; //answer;
                    Console.WriteLine(@"Клиент: {0}	Запрос: {1}	Время выполнения: {2}", _number, i, queryTime);

                    _config.LogStats.ClientNumber = _number;
                    _config.LogStats.ClientQueryNumber = i;
                    _config.LogStats.QueryNumber = _queryNumber;
                    _config.LogStats.QueryTime = queryTime;

                    Logger.WriteCsv(_config.LogStats);
                }
            }
            tcpClient.Close();
            Console.WriteLine(@"Клиент: {0}	Общее время работы: {1}", _number, _clientStatsData.WaitTime);
        }
    }
}
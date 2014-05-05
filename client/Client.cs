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
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;

namespace client
{
    class Client
    {
        private string _address;
        private int _port;
        private int _number;
        public ClientStatsData ClientStatsData;
        private int _queryNumber;

        public event Action EndWork;

        public Client(string address, int port, int number, int queryNumber)
        {
            _address = address;
            _port = port;
            _number = number;
            _queryNumber = queryNumber;
            Thread t = new Thread(ClientThread);
            t.Start();
        }

        void ClientThread()
        {
            ClientStatsData = new ClientStatsData();
            DateTime startTime;

            var tcpClient = new TcpClient();
            tcpClient.Connect(_address, _port);
            if (tcpClient.Connected)
            {
                string query = Properties.Resources.ResourceManager.GetString("q" + _queryNumber);
                var dbRequestPacket = new DbRequestPacket(query);
                Byte[] requestPacket = dbRequestPacket.GetPacket().ToBytes();
                tcpClient.GetStream().Write(requestPacket, 0, requestPacket.Length);

                startTime = DateTime.Now;

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

                ClientStatsData.WaitTime = DateTime.Now - startTime;
                ClientStatsData.Answer = null;//answer;
            }
            tcpClient.Close();
            Console.WriteLine(_number + "\t" + ClientStatsData.WaitTime);
            if (EndWork != null) EndWork();
        }
    }
}

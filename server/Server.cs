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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using server.DataBase;

namespace server
{
    internal class Server
    {
        private readonly TcpListener _listener;

        private readonly TcpClient _tcpClient;

        private bool serverIsLife;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (serverIsLife)
            {
                _tcpClient = _listener.AcceptTcpClient();

                Logger.Write("Принято соединие");

                while (_tcpClient.Connected)
                {
                    int workerThreads, completionPortThreads;
                    ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
                    bool status = (workerThreads <= Environment.ProcessorCount*2);
                    var sp = new StatusPacket(status);
                    Byte[] statusPacket = sp.GetPacket().ToBytes();
                    try
                    {
                        if (_tcpClient.Connected)
                            _tcpClient.GetStream().Write(statusPacket, 0, statusPacket.Length);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Исключение при отсылке статуса: " + ex.Message);
                    }
                    Logger.Write("Отослан статус " + status.ToString());

                    string packetData = "";
                    var buffer = new byte[1400];

                    try
                    {
                        int count;
                        while ((count = _tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)

                        {
                            packetData += Encoding.ASCII.GetString(buffer, 0, count);
                            if (packetData.IndexOf("\n\r", StringComparison.Ordinal) >= 0) break;
                        }
                        ThreadPool.QueueUserWorkItem(MySqlWorker, packetData);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Исключение при чтении запроса: "+ ex.Message);
                    }
                }
            }
        }

        private void MySqlWorker(object data)
        {
            var packetData = (string) data;
            var packet = new Packet(packetData);

            Logger.Write("Принят запрос: " + packet.Data);

            DataTable dt = null;
            if (packet.Type == PacketType.Request)
            {
                Logger.Write("Запрос начал выполнение");
                var sw = new Stopwatch();
                sw.Start();
                try
                {
                    dt = DB.CustomQuery(packet.Data);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                }
                sw.Stop();
                Logger.Write("Запрос выполнен за " + sw.ElapsedMilliseconds + " мс");
            }
            if (dt != null)
            {
                Logger.Write("Отправка результата клиенту");
                var dbAnswerPacket = new DbAnswerPacket(dt);
                Packet answerPacket = dbAnswerPacket.GetPacket();
                answerPacket.Type = PacketType.Answer;
                answerPacket.ClientId = packet.ClientId;
                answerPacket.Id = packet.Id;
                Byte[] answerPacketBytes = answerPacket.ToBytes();
                _tcpClient.GetStream().Write(answerPacketBytes, 0, answerPacketBytes.Length);
            }
        }

        /// <summary>
        /// убийца - деструктор
        /// </summary>
        ~Server()
        {
            serverIsLife = false;
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
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
using server.Config.Data;
using server.DataBase;

namespace server
{
    /// <summary>
    ///     СЕРВЕР БД
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Слушатель новых соедиений
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        ///     текущее содинение
        /// </summary>
        private readonly TcpClient _tcpClient;

        /// <summary>
        ///     длина очереди на сервере
        /// </summary>
        private int _queueLength;

        /// <summary>
        ///     признак жизни потока слушателя
        /// </summary>
        private bool _serverIsLife;

        public Server(uint port)
        {
            _listener = new TcpListener(IPAddress.Any, (int)port);

            _listener.Start();

            _serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                _tcpClient = _listener.AcceptTcpClient();

                Logger.Write("Принято соединие");
                string packetData = "";

                while (_tcpClient.Connected)
                {
                    SendStatus();
                    Packet onePacketData = Balancer.Common.Utils.PacketTransmitHelper.Recive(_tcpClient.GetStream());
                    _queueLength++;
                    SendStatus();
                    ThreadPool.QueueUserWorkItem(MySqlWorker, onePacketData);
                    //var buffer = new byte[1400];
                    //
                    //try
                    //{
                    //    int count;
                    //    while ((count = _tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    //    {
                    //        packetData += Encoding.ASCII.GetString(buffer, 0, count);
                    //        if (packetData.Contains(Packet.PacketEnd)) break;
                    //    }
                    //    while (packetData.Contains(Packet.PacketEnd))
                    //    {
                    //        int index = packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal);
                    //        string onePacketData = packetData.Remove(index);
                    //        packetData =
                    //            packetData.Substring(
                    //                index +
                    //                Packet.PacketEnd.Length);
                    //
                    //        _queueLength++;
                    //        SendStatus();
                    //        ThreadPool.QueueUserWorkItem(MySqlWorker, onePacketData);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Logger.Write("Исключение при чтении запроса: " + ex.Message);
                    //}


                }
            }
        }

        /// <summary>
        ///     Отправка статуса сервера
        /// </summary>
        private void SendStatus()
        {
            bool status = (_queueLength <= Environment.ProcessorCount*2);
            var sp = new StatusPacket(status);
            Balancer.Common.Utils.PacketTransmitHelper.Send(sp.GetPacket(), _tcpClient.GetStream());
            Logger.Write("Отослан статус " + status);
        }

        /// <summary>
        ///     Обработчик запроса в MySQL
        /// </summary>
        /// <param name="packet"></param>
        private void MySqlWorker(object packetObj)
        {
            DataTable dt = null;
            Packet packet = (Packet) packetObj;
            var requestPacket = new DbRequestPacket(packet.Data);

            Logger.Write("Принят запрос от клиента: " + requestPacket.ClientId);
            if (packet.Type == PacketType.Request)
            {
                dt = ProcessQuery(requestPacket);
            }
            if (dt != null)
            {
                Logger.Write("Отправка результата клиенту " + requestPacket.ClientId);
                var dbAnswerPacket = new DbAnswerPacket(dt, requestPacket.QueryNumber, new PacketBase { ClientId = requestPacket.ClientId, RegionId = requestPacket.RegionId});
                Balancer.Common.Utils.PacketTransmitHelper.Send(dbAnswerPacket.GetPacket(), _tcpClient.GetStream());
            }
            _queueLength--;
            SendStatus();
        }

        private DataTable ProcessQuery(DbRequestPacket requestPacket)
        {
            DataTable dt = null;
            Logger.Write("Запрос начал выполнение");
            var sw = new Stopwatch();
            sw.Start();

            switch (Config.ServerConfig.Instance.Server.WorkMode)
            {
                case WorkMode.Normal:
                    dt = ProcessQueryWithMySQL(requestPacket);
                    break;
                case WorkMode.Simulation:
                    dt = ProcessQuerySimulated(requestPacket);
                    break;
            }

            sw.Stop();
            Logger.Write("Запрос " + requestPacket.QueryNumber + " выполнен за " + sw.ElapsedMilliseconds + " мс");
            return dt;
        }

        private DataTable ProcessQueryWithMySQL(DbRequestPacket requestPacket)
        {
            DataTable dt=null;
            try
            {
                dt = Database.CustomQuery(requestPacket.Query);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            return dt;
        }

        private DataTable ProcessQuerySimulated(DbRequestPacket requestPacket)
        {
            Thread.Sleep(Config.ServerConfig.Instance.SimulatedTimes[requestPacket.QueryNumber]);
            var dt = new DataTable {TableName = "randomTable"};
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            return dt; 
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
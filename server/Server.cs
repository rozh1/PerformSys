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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Packet;
using PerformSys.Common.Packet.Packets;
using PerformSys.Common.Utils;
using server.Config;
using server.Config.Data;
using server.Config.Data.LogData;
using server.DataBase;

namespace server
{
    /// <summary>
    ///     СЕРВЕР БД
    /// </summary>
    internal class Server
    {
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
        private bool _serverIsLife = true;

        private readonly Dictionary<int, MySqlDb> _databases;

        private PacketTransmitHelper _transmitHelper;

        public Server(Dictionary<int,MySqlDb> databases)
        {
            _transmitHelper = new PacketTransmitHelper(ServerConfig.Instance.Log.LogFile);
            _databases = databases;
            _tcpClient = new TcpClient();
            while (_serverIsLife)
            {
                try
                {
                    _tcpClient.Connect(
                        ServerConfig.Instance.Server.RBN.Host, 
                        (int)ServerConfig.Instance.Server.RBN.Port);
                }
                catch (Exception)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile, 
                        new StringLogData("Не удалось подключиться. Переподключение"), 
                        LogLevel.ERROR);
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("Подключение установлено"), 
                    LogLevel.INFO);

                foreach (Config.Data.DataBase dataBase in ServerConfig.Instance.DataBase)
                {
                    SendDataBaseInfo(dataBase.RegionId);
                }

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
                        Logger.Write(ServerConfig.Instance.Log.LogFile,
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
                        Logger.Write(ServerConfig.Instance.Log.QueueStatsFile,
                            new QueueStats(packetBase.GlobalId, packetBase.RegionId, (int) packetBase.ClientId,
                                _queueLength),
                            LogLevel.INFO);

                        SendStatus();
                        ThreadPool.QueueUserWorkItem(MySqlWorker, onePacketData);
                    }
                    else
                    {
                        Logger.Write(ServerConfig.Instance.Log.LogFile, 
                            new StringLogData("Получен пустой пакет или разорвано соединение"), 
                            LogLevel.ERROR);
                    }
                }
                _tcpClient.Close();
                _tcpClient = new TcpClient();
            }
        }

        void SendDataBaseInfo(int regionId)
        {
            DataTable dt = null;
            var requestPacket = new DbRequestPacket("SELECT table_name AS table_name, data_length FROM information_schema.tables WHERE table_schema=DATABASE();", 0)
            {
                GlobalId = 0,
                RegionId = (uint)regionId,
            };
            switch (ServerConfig.Instance.Server.WorkMode)
            {
                case WorkMode.Normal:
                    dt = ProcessQueryWithMySql(requestPacket);
                    break;
                case WorkMode.Simulation:
                    dt = GenerateSimulatedSizesDataTable(requestPacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Нет такого режима работы");
            }

            var tableSizes = new Dictionary<string, UInt64>();
            foreach (DataRow row in dt.Rows)
            {
                if (!tableSizes.ContainsKey(row["table_name"].ToString()))
                    tableSizes.Add(row["table_name"].ToString(), Convert.ToUInt64(row["data_length"]));
            }

            if (dt != null)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData(string.Format("Отправка информации о {0} БД РБНу",regionId)), 
                    LogLevel.INFO);
                
                var dataBaseInfoPacket = new DataBaseInfoPacket(tableSizes);
                dataBaseInfoPacket.RegionId = (uint)regionId;

                try
                {
                    _transmitHelper.Send(dataBaseInfoPacket.GetPacket(), _tcpClient.GetStream());
                }
                catch (Exception ex)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile,
                       new StringErorrLogData(
                           "{0}. Ошибка передачи ответа: {1}",
                           System.Reflection.MethodBase.GetCurrentMethod().Name,
                           ex.Message),
                       LogLevel.ERROR);
                }
            }
        }

        private DataTable GenerateSimulatedSizesDataTable(DbRequestPacket requestPacket)
        {
            var dt = new DataTable {TableName = "sizes"};
            dt.Columns.Add("table_name");
            dt.Columns.Add("data_length");

            var currentDataBase = ServerConfig.Instance.DataBase.FirstOrDefault(
                curDataBase => curDataBase.RegionId == requestPacket.RegionId);
            if (currentDataBase != null)
            {
                Dictionary<string, UInt64> sizes =
                    currentDataBase.SimulationSizes;

                if (sizes != null)
                    foreach (KeyValuePair<string, UInt64> pair in sizes)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = pair.Key;
                        dr[1] = pair.Value;
                        dt.Rows.Add(dr);
                    }
            }
            return dt;
        }

        /// <summary>
        ///     Отправка статуса сервера
        /// </summary>
        private void SendStatus()
        {
            bool status = (_queueLength < Environment.ProcessorCount+1);
            var sp = new ServerStatusPacket(status);
            if (_tcpClient.Connected)
            {
                _transmitHelper.Send(sp.GetPacket(), _tcpClient.GetStream());
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("Отослан статус " + status), 
                    LogLevel.INFO);
            }
        }

        /// <summary>
        ///     Обработчик запроса в MySQL
        /// </summary>
        /// <param name="packetObj"></param>
        private void MySqlWorker(object packetObj)
        {
            DataTable dt = null;
            var packet = (Packet) packetObj;
            var requestPacket = new DbRequestPacket(packet.Data);
            var elapsedTime = new TimeSpan();
            long packetLength = 0;

            Logger.Write(ServerConfig.Instance.Log.LogFile, 
                new StringLogData("Принят запрос от клиента: " + requestPacket.ClientId), 
                LogLevel.INFO);
            if (packet.Type == PacketType.Request)
            {
                dt = ProcessQuery(requestPacket, out elapsedTime);
            }
            if (dt != null)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("Отправка результата клиенту " + requestPacket.ClientId), 
                    LogLevel.INFO);
                var dbAnswerPacket = new DbAnswerPacket(
                    Encoding.UTF8.GetBytes(SerializeMapper.Serialize(dt)), 
                    requestPacket.QueryNumber,
                    new PacketBase {ClientId = requestPacket.ClientId, RegionId = requestPacket.RegionId, GlobalId = requestPacket.GlobalId});
                packetLength = dbAnswerPacket.GetPacket().ToBase64String().Length;
                Logger.Write(ServerConfig.Instance.Log.LogFile,
                    new StringLogData(string.Format("Размер посылки ответа запроса {0}: {1} ", requestPacket.QueryNumber, packetLength)), 
                    LogLevel.INFO);
                try
                {
                    _transmitHelper.Send(dbAnswerPacket.GetPacket(), _tcpClient.GetStream());
                }
                catch (Exception ex)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile,
                       new StringErorrLogData(
                           "{0}. Ошибка передачи ответа: {1}",
                           System.Reflection.MethodBase.GetCurrentMethod().Name,
                           ex.Message),
                       LogLevel.ERROR);
                }
            }

            Logger.Write(ServerConfig.Instance.Log.StatsFile,
                new Stats(
                    requestPacket.GlobalId,
                    requestPacket.RegionId,
                    (int) requestPacket.ClientId,
                    requestPacket.QueryNumber,
                    elapsedTime,
                    packetLength,
                    _queueLength
                    ), LogLevel.INFO);

            _queueLength--;
            SendStatus();

            Logger.Write(ServerConfig.Instance.Log.QueueStatsFile,
                            new QueueStats(requestPacket.GlobalId, requestPacket.RegionId, (int)requestPacket.ClientId,
                                _queueLength),
                            LogLevel.INFO);
        }

        /// <summary>
        ///     Обработчик запросов
        /// </summary>
        /// <param name="requestPacket">пакет запроса</param>
        /// <param name="elapsedTime">Время работы</param>
        /// <returns></returns>
        private DataTable ProcessQuery(DbRequestPacket requestPacket, out TimeSpan elapsedTime)
        {
            DataTable dt;
            Logger.Write(ServerConfig.Instance.Log.LogFile,
                new StringLogData("Запрос начал выполнение"), 
                LogLevel.INFO);
            var startTime = DateTime.UtcNow;

            switch (ServerConfig.Instance.Server.WorkMode)
            {
                case WorkMode.Normal:
                    dt = ProcessQueryWithMySql(requestPacket);
                    break;
                case WorkMode.Simulation:
                    dt = ProcessQuerySimulated(requestPacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ServerConfig.Instance.Server.WorkMode Нет такого режима работы");
            }

            elapsedTime = DateTime.UtcNow - startTime;
            Logger.Write(ServerConfig.Instance.Log.LogFile,
                new StringLogData("Запрос " + requestPacket.QueryNumber + " выполнен за " + elapsedTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture) + " мс"), 
                LogLevel.INFO);
            return dt;
        }

        /// <summary>
        ///     Обрабочик запроса с помощью MySQL
        /// </summary>
        /// <param name="requestPacket">пакет запроса</param>
        /// <returns></returns>
        private DataTable ProcessQueryWithMySql(DbRequestPacket requestPacket)
        {
            bool getFullResult = true;
            int region = (int)requestPacket.RegionId;
            DataTable dt = null;

            var currentDataBase = ServerConfig.Instance.DataBase.FirstOrDefault(
                db => db.RegionId == region);

            if (currentDataBase != null)
                getFullResult = currentDataBase.GetFullQueryResult;

            try
            {
                dt = _databases[region].Select(requestPacket.Query, getFullResult).Tables[0];
            }
            catch (Exception ex)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile,
                    new StringLogData(ex.Message), 
                    LogLevel.ERROR);
            }
            return dt;
        }

        /// <summary>
        ///     Симулятор обрботки запросов
        /// </summary>
        /// <param name="requestPacket">пакет запроса</param>
        /// <returns></returns>
        private DataTable ProcessQuerySimulated(DbRequestPacket requestPacket)
        {
            var rand = new Random();

            var dt = new DataTable {TableName = "randomTable"};
            dt.Columns.Add("Data");

            var currentDataBase = ServerConfig.Instance.DataBase.FirstOrDefault(
                simParam => simParam.RegionId == requestPacket.RegionId);

            if (currentDataBase != null)
            {
                Dictionary<int, int[]> simuldationParams =
                    currentDataBase.SimulationParams;
            
                if (simuldationParams != null)
                {
                    Thread.Sleep(simuldationParams[requestPacket.QueryNumber][0]);
                    var data = new byte[simuldationParams[requestPacket.QueryNumber][1]/2];

                    rand.NextBytes(data);

                    DataRow dr = dt.NewRow();
                    dr[0] = Convert.ToBase64String(data);
                    dt.Rows.Add(dr);
                    return dt;
                }
            }

            Logger.Write(ServerConfig.Instance.Log.LogFile,
                new StringLogData("Нет параметров симуляции. Длина пакета минимальная."), 
                LogLevel.INFO);
            DataRow dataRow = dt.NewRow();
            dataRow[0] = "";
            dt.Rows.Add(dataRow);
            return dt;
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
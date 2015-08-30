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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using MySql.Data.MySqlClient;
using server.Config;

namespace server.DataBase
{
    internal class MySqlDb
    {
        /// строка параметров подключения
        private readonly MySqlConnectionStringBuilder _connectionString;

        private string _database;
        private int _comandTimeout = int.MaxValue;

        public MySqlDb(string user, string password, string database, string server, uint port)
        {
            _connectionString = new MySqlConnectionStringBuilder
            {
                UserID = user,
                Password = password,
                Server = server,
                Database = database,
                Port = port,
                MaximumPoolSize = 50,
                MinimumPoolSize = 2,
                Pooling = true
                
            };
            _database = database;
        }

        public event Action ConnectionError;

        /// <summary>
        ///     Установка соединения с MySQL сервером
        /// </summary>
        public bool MySqlConnectionOpen()
        {
            bool result = true;
            try
            {
                var conn = new MySqlConnection(_connectionString.GetConnectionString(true));
                conn.Open();
                Insert("SET NAMES utf8", conn);
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("Выполненно подключение к БД " + _database), 
                    LogLevel.INFO);
                conn.Close();
            }
            catch (MySqlException e)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("При подключении к серверу MySQL возникло исколючение: " + e.Message), 
                    LogLevel.INFO);
                result = false;
            }
            return result;
        }

        public MySqlConnection ConnectionOpen()
        {
            var conn = new MySqlConnection(_connectionString.GetConnectionString(true));
            try
            {
                conn.Open();
                Insert("SET NAMES utf8", conn);
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("Выполненно подключение к БД"), 
                    LogLevel.INFO);
            }
            catch (MySqlException e)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile,
                    new StringLogData("При подключении к серверу MySQL возникло исколючение: " + e.Message),
                    LogLevel.ERROR);
                if (ConnectionError != null) ConnectionError();
            }
            return conn;
        }

        public void ConnectionClose(MySqlConnection conn)
        {
            conn.Close();
        }

        /// <summary>
        ///     Вставка строки в таблицу по запросу
        /// </summary>
        /// <param name="query">запрос к БД</param>
        public void Insert(string query)
        {
            //if (mysqlConn.State != System.Data.ConnectionState.Open) MySQLConnectionOpen();
            using (MySqlConnection conn = ConnectionOpen())
            {
                var cmd = new MySqlCommand(query, conn);
                cmd.CommandTimeout = _comandTimeout;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile, 
                        new StringLogData("При записи в БД произошло исключение: " + e.Message), 
                        LogLevel.ERROR);
                }
                ConnectionClose(conn);
            }
        }

        public void Insert(string query, MySqlConnection conn)
        {
            var cmd = new MySqlCommand(query, conn);
            cmd.CommandTimeout = _comandTimeout;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile, 
                    new StringLogData("При записи в БД произошло исключение: " + e.Message), 
                    LogLevel.ERROR);
            }
        }

        /// <summary>
        ///     Выборка из БД по запросу
        /// </summary>
        /// <param name="query">запрос</param>
        /// <param name="getFullResult">
        /// <para>true - получить весь ответ от MySQL</para>
        /// <para>false - выполнение запроса без получения данных от MySQL</para>
        /// </param>
        /// <returns>структура DataSet с ответом</returns>
        public DataSet Select(string query, bool getFullResult = true)
        {
            //if (mysqlConn.State != System.Data.ConnectionState.Open) MySQLConnectionOpen();
            var answer = new DataSet();
            using (MySqlConnection conn = ConnectionOpen())
            {
                try
                {
                    var cmd = new MySqlCommand(query, conn);
                    cmd.CommandTimeout = _comandTimeout;
                    if (getFullResult)
                    {
                        var da = new MySqlDataAdapter(cmd);
                        da.Fill(answer, "Answer");
                        da.Dispose();
                    }
                    else
                    {
                        cmd.ExecuteScalar();
                        answer.Tables.Add(new DataTable("Answer"));
                    }
                }
                catch (Exception e)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile, 
                        new StringLogData("При чтении из БД произошло исключение: " + e.Message), 
                        LogLevel.ERROR);
                }
                ConnectionClose(conn);
            }
            return answer;
        }

        protected List<DbDataRecord> GetData(MySqlCommand command)
        {
            var dataList = new List<DbDataRecord>();
            using (command)
            using (var connection = ConnectionOpen())
            {
                try
                {
                    connection.Open();
                    command.Connection = connection;
                    using (MySqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            foreach (DbDataRecord record in dataReader)
                            {
                                dataList.Add(record);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile, 
                        new StringLogData("GetData. При получении данных из БД вылетело исключение: " + ex.Message),
                        LogLevel.ERROR);
                }
            }
            return dataList;
        }

        /// <summary>
        ///     Обновление записи по запросу
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>Количество затронутых строк</returns>
        public int Update(string query)
        {
            //if (mysqlConn.State != System.Data.ConnectionState.Open) MySQLConnectionOpen();
            int updatedRows = 0;
            using (MySqlConnection conn = ConnectionOpen())
            {
                var cmd = new MySqlCommand(query, conn);
                cmd.CommandTimeout = _comandTimeout;
                try
                {
                    updatedRows = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile, 
                        new StringLogData("При обновлении записи в БД произошло исключение: " + e.Message), 
                        LogLevel.ERROR);
                }
                ConnectionClose(conn);
            }
            return updatedRows;
        }

        /// <summary>
        ///     Удаление записи по запросу
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>Количество затронутых строк</returns>
        public int Delete(string query)
        {
            //if (mysqlConn.State != System.Data.ConnectionState.Open) MySQLConnectionOpen();
            int deletedRows = 0;
            using (MySqlConnection conn = ConnectionOpen())
            {
                var cmd = new MySqlCommand(query, conn);
                cmd.CommandTimeout = _comandTimeout;
                try
                {
                    deletedRows = cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Logger.Write(ServerConfig.Instance.Log.LogFile, 
                        new StringLogData("При удалении записи из БД произошло исключение: " + e.Message), 
                        LogLevel.ERROR);
                }
                ConnectionClose(conn);
            }
            return deletedRows;
        }
    }
}
﻿using System;
using System.Data;
using Balancer.Common.Logger;
using MySql.Data.MySqlClient;

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
                Logger.Write("Выполненно подключение к БД " + _database, 7);
                conn.Close();
            }
            catch (MySqlException e)
            {
                Logger.Write("При подключении к серверу MySQL возникло исколючение: " + e.Message);
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
                Logger.Write("Выполненно подключение к БД", 7);
            }
            catch (MySqlException e)
            {
                Logger.Write("При подключении к серверу MySQL возникло исколючение: " + e.Message);
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
                    Logger.Write("При записи в БД произошло исключение: " + e.Message);
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
                Logger.Write("При записи в БД произошло исключение: " + e.Message);
            }
        }

        /// <summary>
        ///     Выборка из БД по запросу
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>структура DataSet с ответом</returns>
        public DataSet Select(string query)
        {
            //if (mysqlConn.State != System.Data.ConnectionState.Open) MySQLConnectionOpen();
            var answer = new DataSet();
            using (MySqlConnection conn = ConnectionOpen())
            {
                try
                {
                    var cmd = new MySqlCommand(query, conn);
                    cmd.CommandTimeout = _comandTimeout;
                    var da = new MySqlDataAdapter(cmd);
                    da.Fill(answer, "Answer");
                    da.Dispose();
                }
                catch (Exception e)
                {
                    Logger.Write("При чтении из БД произошло исключение: " + e.Message);
                }
                ConnectionClose(conn);
            }
            return answer;
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
                    Logger.Write("При обновлении записи в БД произошло исключение: " + e.Message);
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
                    Logger.Write("При удалении записи из БД произошло исключение: " + e.Message);
                }
                ConnectionClose(conn);
            }
            return deletedRows;
        }
    }
}
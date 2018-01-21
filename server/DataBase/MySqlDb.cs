using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
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
            var retry = 0;
            bool result = true;
            string errMsg = string.Empty;
            MySqlConnection conn;
            do
            {
                conn = new MySqlConnection(_connectionString.GetConnectionString(true));
                try
                {
                    conn.Open();
                    Insert("SET NAMES utf8", conn);
                    Logger.Write(ServerConfig.Instance.Log.LogFile,
                        new StringLogData("Выполненно подключение к БД"),
                        LogLevel.INFO);
                    result = true;
                }
                catch (MySqlException e)
                {
                    retry++;
                    Logger.Write(ServerConfig.Instance.Log.LogFile,
                        new StringLogData("Попытка подключения: " + retry + " из 10"),
                        LogLevel.INFO);
                    result = false;
                    errMsg = e.Message;
                }

            } while (!result && retry < 10);
            if (!result)
            {
                Logger.Write(ServerConfig.Instance.Log.LogFile,
                    new StringLogData("При подключении к серверу MySQL возникло исколючение: " + errMsg),
                    LogLevel.ERROR);
                ConnectionError?.Invoke();
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
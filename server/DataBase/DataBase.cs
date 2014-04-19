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
using System.Globalization;
using Balancer.Common;

namespace server.DataBase
{
    /// <summary>
    ///     Работа с БД
    /// </summary>
    public static class DB
    {
        private static MySqlDb mysqldb;

        private static string database = "";

        private static NumberFormatInfo nfi;

        public static event Action ConnectionError;

        /// <summary>
        ///     Инициализация класса
        /// </summary>
        /// <returns>true в случае успеха</returns>
        public static bool Init()
        {
            database = ConfigFile.GetConfigValue("DataBase");
            nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            switch (database)
            {
                case "mysql":
                    mysqldb = new MySqlDb(
                        ConfigFile.GetConfigValue("MySQL_user"),
                        ConfigFile.GetConfigValue("MySQL_password"),
                        ConfigFile.GetConfigValue("MySQL_database"),
                        ConfigFile.GetConfigValue("MySQL_server"),
                        ConfigFile.GetConfigValue("MySQL_port"));
                    mysqldb.ConnectionError += mysqldb_ConnectionError;
                    return mysqldb.MySqlConnectionOpen();
            }
            return false;
        }

        private static void mysqldb_ConnectionError()
        {
            if (ConnectionError != null) ConnectionError();
        }

        /// <summary>
        ///     Проверка инициализации
        /// </summary>
        private static void CheckInit()
        {
            switch (database)
            {
                case "mysql":
                    if (mysqldb == null) Init();
                    break;
            }
        }


        /// <summary>
        ///     Запрос Вставки
        /// </summary>
        /// <param name="query">Запрос</param>
        private static void InsertQuery(string query)
        {
            CheckInit();
            //Logger.Write("Запрос на запись к базе: " + query, 7);
            switch (database)
            {
                case "mysql":
                    mysqldb.Insert(query);
                    break;
            }
        }

        /// <summary>
        ///     Запрос выборки из БД
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>результаты запроса</returns>
        private static DataSet SelectQuery(string query)
        {
            CheckInit();
            //Logger.Write("Запрос на чтение к базе: " + query, 7);
            switch (database)
            {
                case "mysql":
                    return mysqldb.Select(query);
            }
            return null;
        }

        /// <summary>
        ///     Запрос на обновление в БД
        /// </summary>
        /// <param name="query">запрос</param>
        private static void UpdateQuery(string query)
        {
            CheckInit();
            //Logger.Write("Запрос на обновление к базе: " + query, 7);
            int updatedRows = 0;
            switch (database)
            {
                case "mysql":
                    updatedRows = mysqldb.Update(query);
                    break;
            }
            Logger.Write("В результате измененно " + updatedRows + " строк", 7);
        }

        /// <summary>
        ///     Завпрос удаления из БД
        /// </summary>
        /// <param name="query">запрос</param>
        private static void DeleteQuery(string query)
        {
            CheckInit();
            Logger.Write("Запрос на удаление к базе: " + query, 7);
            int deletedRows = 0;
            switch (database)
            {
                case "mysql":
                    deletedRows = mysqldb.Delete(query);
                    break;
            }
            Logger.Write("В результате измененно " + deletedRows + " строк", 7);
        }

        /// <summary>
        ///     Произвольный запрос
        /// </summary>
        /// <param name="query">запрос</param>
        /// <returns>результат select или null</returns>
        public static DataTable CustomQuery(string query)
        {
            string str = query.Substring(0, 10);
            if (str.Contains("SELECT") || str.Contains("select") || str.Contains("Select"))
                return SelectQuery(query).Tables[0];
            InsertQuery(query);
            return null;
        }
    }
}
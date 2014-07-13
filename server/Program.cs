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
using System.IO;
using System.Text;
using System.Threading;
using Balancer.Common;
using server.Config;
using server.Config.Data;
using server.DataBase;
using server.Properties;

namespace server
{
    internal class Program
    {
        private static void Main()
        {
            Logger.SetLogFile("serverLog.txt");
            Logger.Write("Сервер запущен");

            const string configFilePath = "serverConfig.xml";
            if (!File.Exists(configFilePath))
            {
                ServerConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            ServerConfig.Load(configFilePath);

            int maxThreadsCount = Environment.ProcessorCount;

            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            ThreadPool.SetMinThreads(1, 1);

            var databases = new Dictionary<int, MySqlDb>();

            if (ServerConfig.Instance.Server.WorkMode == WorkMode.Normal)
            {
                foreach (var dataBaseConfig in ServerConfig.Instance.DataBase)
                {
                    var database = new MySqlDb(
                        dataBaseConfig.UserName,
                        dataBaseConfig.Password,
                        dataBaseConfig.DataBaseName,
                        dataBaseConfig.Host,
                        dataBaseConfig.Port);

                    if (!database.MySqlConnectionOpen())
                    {
                        Logger.Write("Ошибка подключения к БД. Выход.");
                        return;
                    }
                    databases.Add(dataBaseConfig.RegionId, database);
                }
            }

            Logger.Write("Сервер конфигурирован");
            new Server(databases);
        }
    }
}
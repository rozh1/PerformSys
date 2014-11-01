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
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using server.Config;
using server.Config.Data;
using server.DataBase;

namespace server
{
    internal static class Program
    {
        private static void Main()
        {
            const string configFilePath = "serverConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            ServerConfig.Load(configFilePath);

            Logger.Configure(ServerConfig.Instance.Log);
            Logger.Write(ServerConfig.Instance.Log.LogFile, 
                new StringLogData("Сервер запущен"), 
                LogLevel.INFO);

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
                        Logger.Write(ServerConfig.Instance.Log.LogFile, 
                            new StringLogData("Ошибка подключения к БД. Выход."), 
                            LogLevel.FATAL);
                        return;
                    }
                    databases.Add(dataBaseConfig.RegionId, database);
                }
            }

            Logger.Write(ServerConfig.Instance.Log.LogFile, 
                new StringLogData("Сервер конфигурирован"), 
                LogLevel.INFO);
            new Server(databases);
        }
    }
}
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Utils.CommandLineArgsParser;
using server.Config;
using server.Config.Data;
using server.DataBase;

namespace server
{
    internal static class Program
    {
        private static void Main(string[] args)
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
                foreach (Config.Data.DataBase dataBaseConfig in ServerConfig.Instance.DataBase)
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

            var comandLineArguments = new List<ComandLineArgument>
            {
                new ComandLineArgument("--host", new[] {"-h"}),
                new ComandLineArgument("--port", new[] {"-p"})
            };

            var commandLineArgumentsParser = new CommandLineArgumentsParser(comandLineArguments.ToArray());
            ComandLineArgument[] parseResult = commandLineArgumentsParser.Parse(args);

            foreach (ComandLineArgument comandLineArgument in parseResult)
            {
                if (comandLineArgument.IsDefined)
                {
                    switch (comandLineArgument.Argument)
                    {
                        case "--host":
                            ServerConfig.Instance.Server.RBN.Host = comandLineArgument.Value;
                            break;
                        case "--port":
                            int port;
                            if (int.TryParse(comandLineArgument.Value, out port))
                            {
                                ServerConfig.Instance.Server.RBN.Port = (uint) port;
                            }
                            break;
                    }
                }
            }

            new Server(databases);
        }
    }
}
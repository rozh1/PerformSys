﻿#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen, Lenar Khisamiev
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
using System.IO;
using System.Linq;
using client.Config;
using client.QuerySequence;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Utils.CommandLineArgsParser;

namespace client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Init(args);

            var scenario = ClientConfig.Instance.Scenario;
            var querySequenceManager = new QuerySequenceManager(ClientConfig.Instance);
            var clients = new Client[scenario.ClientCount];

            Logger.Write(ClientConfig.Instance.Log.LogFile, new StringLogData("Запуск клиентов..."), LogLevel.INFO);

            int queriesCount = ClientConfig.Instance.Queries.Count;
            for (int i = 0; i < scenario.ClientCount; i++)
            {
                var qSeq = querySequenceManager.GetQuerySequence((i % queriesCount) + 1, queriesCount);
                clients[i] = new Client(ClientConfig.Instance, (i + 1), qSeq);
            }

            Logger.Write(ClientConfig.Instance.Log.LogFile, new StringLogData("Клиенты запущены"), LogLevel.INFO);
        }

        private static void Init(string[] args)
        {
            const string configFilePath = "clientConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            ClientConfig.Load(configFilePath);

            var cmdArgs = new ComandLineArgument[]
            {
                new ComandLineArgument("--queryList", new []{"-q"}, "Список запросов", false),
                new ComandLineArgument("--host", new []{"-h"}, "Адрес балансировщика", false),
                new ComandLineArgument("--port", new []{"-p"}, "Порт балансировщика", false),
                new ComandLineArgument("--help", "Справка", false)
            };

            var commandLineArgumentsParser = new CommandLineArgumentsParser(cmdArgs.ToArray());
            ComandLineArgument[] parseResult = commandLineArgumentsParser.Parse(args);

            foreach (var comandLineArgument in parseResult)
            {
                if (!comandLineArgument.IsDefined && comandLineArgument.IsRequired)
                {
                    Console.WriteLine("ОШИБКА: не указан аргумент {0} - {1}", comandLineArgument.Argument,
                        comandLineArgument.Description);
                    Environment.Exit(100);
                }
            }

            string queryListPath = "queries.xml";
            foreach (var comandLineArgument in parseResult)
            {
                if (comandLineArgument.IsDefined)
                {
                    switch (comandLineArgument.Argument)
                    {
                        case "--help":
                            foreach (var descr in commandLineArgumentsParser.GetDescriptions())
                            {
                                Console.WriteLine(descr);
                            }
                            Environment.Exit(0);
                            break;
                        case "--queryList":
                            queryListPath = comandLineArgument.Value;
                            break;
                        case "--host":
                            ClientConfig.Instance.Server.Host = comandLineArgument.Value;
                            break;
                        case "--port":
                            int port = 0;
                            if (int.TryParse(comandLineArgument.Value, out port))
                            {
                                if (port > 0 && port < 65536)
                                {
                                    ClientConfig.Instance.Server.Port = port;
                                }
                                else
                                {
                                    Console.WriteLine("ОШИБКА: Порт должен быть числом от 1 до 65535!");
                                    Environment.Exit(101);
                                }
                            }
                            else
                            {
                                Console.WriteLine("ОШИБКА: Порт должен быть числом!");
                                Environment.Exit(102);
                            }
                            break;
                    }
                }
            }

            if (!File.Exists(queryListPath))
            {
                (new ConfigInit()).Queries.Save(queryListPath);
            }
            QueriesList.Load(queryListPath);

            ClientConfig.Instance.Queries = QueriesList.Instance.Queries;

            Logger.Configure(ClientConfig.Instance.Log);

        }
    }
}
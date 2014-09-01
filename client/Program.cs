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
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using client.ComandLineParamsParser;
using client.Config;
using client.Config.Data;

namespace client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(@"Использование эмулятора клиентов {0}:", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"{0} --host localhost --port 3409 --scenario scenario.xml",
                    AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"    --host, -h      - адрес балансировщика");
                Console.WriteLine(@"    --port, -p      - порт балансировщика");
                Console.WriteLine(@"    --scenario, -s  - сценарий работы клиентов");
                Console.WriteLine(@"Не обязательные параметры:");
                Console.WriteLine(@"    --log           - имя файла лога [clientLog.txt]");
                Console.WriteLine(@"    --csv           - имя файла статистики [clientStats.csv]");
                Console.WriteLine(@"    --logdir        - папка для лога [текущая]");
                Console.WriteLine(@"    --log-to-console - вывод лога в консоль приложения");
                Console.WriteLine(@"    --default-scenario - создает файл с сценарием по умолчанию");
                Environment.Exit(-1);
            }

            var parser = new Parser(args);

            if (!string.IsNullOrEmpty(parser.ErrorText))
            {
                Console.WriteLine(parser.ErrorText);
                Logger.Write("client.err", new StringLogData(parser.ErrorText), LogLevel.FATAL);
                Environment.Exit(-1);
            }

            Config.Config config = parser.GetConfig();
            config.LogStats = new LogStats();
            Logger.Configure(config.Log);

            Scenario scenario = Scenario.Load(config.ScenarioFile);

            var clients = new Client[scenario.ClientCount];

            Logger.Write(config.Log.LogFile, new StringLogData("Запуск клиентов..."), LogLevel.INFO);

            for (int i = 0; i < scenario.ClientCount; i++)
            {
                var qSeq = new QuerySequence.QuerySequence(14, (i%14) + 1);
                clients[i] = new Client(config, (i + 1), qSeq, scenario.ScenarioSteps);
            }

            Logger.Write(config.Log.LogFile, new StringLogData("Клиенты запущены"), LogLevel.INFO);
        }
    }
}
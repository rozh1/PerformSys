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
using System.Diagnostics;
using Balancer.Common;
using client.ComandLineParamsParser;

namespace client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine(string.Format("Использование эмулятора клиентов {0}:", AppDomain.CurrentDomain.FriendlyName));
                Console.WriteLine(string.Format("{0} --clients 10 --queries 5 --host localhost --port 3409", AppDomain.CurrentDomain.FriendlyName));
                Console.WriteLine(string.Format("\t--clients, -c\t- количество эмулируемых клиентов"));
                Console.WriteLine(string.Format("\t--queries, -q\t- количество запросов от одного клиента"));
                Console.WriteLine(string.Format("\t--host, -h\t- адрес балансировщика"));
                Console.WriteLine(string.Format("\t--port, -p\t- порт балансировщика"));
                Environment.Exit(-1);
            }

            Parser parser = new Parser(args);

            if (!string.IsNullOrEmpty(parser.ErrorText))
            {
                Logger.Write(parser.ErrorText);
                Environment.Exit(-1);
            }

            Config.Config config = parser.GetConfig();

            Debug.Assert(config.ClientCount != null, "config.ClientCount != null");
            var clients = new Client[(int)config.ClientCount];
            
            for (int i = 0; i < (int)config.ClientCount; i++)
            {
                clients[i] = new Client(config, i, (i%14) + 1);
            }
        }
    }
}
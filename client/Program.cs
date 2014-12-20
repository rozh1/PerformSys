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

﻿using System.IO;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using client.Config;
using client.QuerySequence;

namespace client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string configFilePath = "clientConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            ClientConfig.Load(configFilePath);

            Logger.Configure(ClientConfig.Instance.Log);
            var scenario = ClientConfig.Instance.Scenario;
            var querySequenceManager = new QuerySequenceManager(ClientConfig.Instance);
            var clients = new Client[scenario.ClientCount];

            Logger.Write(ClientConfig.Instance.Log.LogFile, new StringLogData("Запуск клиентов..."), LogLevel.INFO);

            int queriesCount = 14;
            for (int i = 0; i < scenario.ClientCount; i++)
            {
                var qSeq = querySequenceManager.GetQuerySequence((i % queriesCount) + 1, queriesCount);
                clients[i] = new Client(ClientConfig.Instance, (i + 1), qSeq);
            }

            Logger.Write(ClientConfig.Instance.Log.LogFile, new StringLogData("Клиенты запущены"), LogLevel.INFO);
        }
    }
}
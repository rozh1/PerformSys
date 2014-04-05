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
using System.IO;
using System.Threading;
using Balancer.Common;

namespace server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.Write("Сервер запущен");

            string configFilePath = "server.log";
            ConfigFile.SetConfigPath(configFilePath);
            if (!File.Exists(configFilePath)) ConfigFile.SaveSettings(Properties.Resources.defaultConfig);
            ConfigFile.LoadSettings();

            int maxThreadsCount = Environment.ProcessorCount*2;

            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            ThreadPool.SetMinThreads(1, 1);

            if (!DataBase.DB.Init())
            {
                Logger.Write("Ошибка подключения к БД. Выход.");
                return;
            }

            Logger.Write("Сервер конфигурирован");
            new Server(int.Parse(ConfigFile.GetConfigValue("ServerPort")));
        }
    }
}
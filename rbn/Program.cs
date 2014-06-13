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
using Balancer.Common;
using rbn.Properties;
using rbn.ServersHandler;

namespace rbn
{
    internal class Program
    {
        private static void Main()
        {
            Logger.Write("Сервер запущен");

            string configFilePath = Environment.CurrentDirectory + "/rbn.cfg";
            ConfigFile.SetConfigPath(configFilePath);
            if (!File.Exists(configFilePath)) ConfigFile.SaveSettings(Resources.defaultConfig);
            ConfigFile.LoadSettings();

            Settings.Init();

            Servers.Init();

            new Server(int.Parse(ConfigFile.GetConfigValue("RBN_Port")));
        }
    }
}
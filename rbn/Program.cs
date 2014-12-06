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
using System.Text;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using rbn.Config;
using rbn.Properties;

namespace rbn
{
    internal static class Program
    {
        private static void Main()
        {
            const string configFilePath = "rbnConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            RBNConfig.Load(configFilePath);

            Logger.Configure(RBNConfig.Instance.Log);
            Logger.Write(RBNConfig.Instance.Log.LogFile,
                new StringLogData("Сервер запущен"), 
                LogLevel.INFO);
            
            new Server((int)RBNConfig.Instance.RBN.Port);
        }
    }
}
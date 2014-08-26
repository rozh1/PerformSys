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

﻿namespace client.ComandLineParamsParser
{
    /// <summary>
    ///     Параметры запуска
    /// </summary>
    internal enum ComandSwitch
    {
        None,
        ClientCount,
        QueryPerClient,
        BalancerHost,
        BalancerPort,
        LogName,
        CsvLogName,
        LogDir,
        WriteLogToConsole
    }

    /// <summary>
    ///     Ключи параметров
    /// </summary>
    internal static class Switchs
    {
        public static ComandSwitch Parse(string input)
        {
            switch (input)
            {
                case "-c":
                case "--clients":
                    return ComandSwitch.ClientCount;
                case "-q":
                case "--queries":
                    return ComandSwitch.QueryPerClient;
                case "-h":
                case "--host":
                    return ComandSwitch.BalancerHost;
                case "-p":
                case "--port":
                    return ComandSwitch.BalancerPort;
                case "--log":
                    return ComandSwitch.LogName;
                case "--csv":
                    return ComandSwitch.CsvLogName;
                case "--logdir":
                    return ComandSwitch.LogDir;
                case "--log-to-console":
                    return ComandSwitch.WriteLogToConsole;
                default:
                    return ComandSwitch.None;
            }
            return ComandSwitch.None;
        }
    }
}
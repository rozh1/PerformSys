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
using Balancer.Common;
using Balancer.Common.Logger;

namespace client.ComandLineParamsParser
{
    /// <summary>
    ///     Обработчик входных параметров
    /// </summary>
    public class Parser
    {
        private readonly string[] _args;
        private readonly Config.Config _config;

        public string ErrorText { get; private set; }

        public Parser(string[] args)
        {
            _config = new Config.Config();
            _args = args;
        }

        /// <summary>
        ///     Парсер параметров
        /// </summary>
        private void Parse()
        {
            for (int i = 0; i < _args.Length - 1; i++)
            {
                ComandSwitch comandSwitch = Switchs.Parse(_args[i]);
                switch (comandSwitch)
                {
                    case ComandSwitch.ClientCount:
                        _config.ClientCount = TryParseInt(_args[i + 1], _args[i]);
                        break;
                    case ComandSwitch.QueryPerClient:
                        _config.QueryCount = TryParseInt(_args[i + 1], _args[i]);
                        break;
                    case ComandSwitch.BalancerHost:
                        _config.BalancerHost = _args[i + 1];
                        break;
                    case ComandSwitch.BalancerPort:
                        _config.BalancerPort = TryParseInt(_args[i + 1], _args[i]);
                        break;
                }
            }
        }

        private int TryParseInt(string str, string key)
        {
            int output;
            if (int.TryParse(str, out output))
                return output;
            Logger.Write("После ключа " + key + " должно следовать число!");
            Environment.Exit(-1);
            return 0;
        }

        /// <summary>
        ///     проверка конфигурации
        /// </summary>
        private void CheckConfig()
        {
            string error = "";
            if (_config.ClientCount == null) error += "Не указано количество клиентов!\n";
            if (_config.QueryCount == null) error += "Не указано количество запросов от одного клиента!\n";
            if (_config.BalancerHost == string.Empty) error += "Не указан адрес балансировщика!\n";
            if (_config.BalancerPort == null) error += "Не указан порт балансировщика!\n";

            ErrorText = error;
        }

        /// <summary>
        ///     Получение конфигурации из строки параметров
        /// </summary>
        /// <returns></returns>
        public Config.Config GetConfig()
        {
            Parse();
            CheckConfig();
            return _config;
        }
    }
}
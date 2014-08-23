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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Helpers.Config
{
    /// <summary>
    /// Класс хелпер, помогающий считать конфигураөионный файл. 
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// Метод для получения значения свойства по ключю.
        /// </summary>
        /// <param name="key">Название ключа.</param>
        /// <returns>Значение свойства.</returns>
        public string GetConfigFromKey(string key)
        {
            string exeConfigPath = this.GetType().Assembly.Location;
            Configuration config = null;
          
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch { }

            if (config != null)
            {
                return GetAppSetting(config, key);
            }
            return null;
        }

        /// <summary>
        /// Метод получения значения свойства из конфигурации.
        /// </summary>
        /// <param name="config">Конфигуриация.</param>
        /// <param name="key">Ключ.</param>
        /// <returns>Значение свойства.</returns>
        private string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }
    }
}

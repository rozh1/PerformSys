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

﻿using BalancerLogger.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Helpers.Config
{
    /// <summary>
    /// Класс для упарвления конфигурационным файлом.
    /// </summary>
    public class ManageConfig
    {
        #region Read

        /// <summary>
        /// Метод загрузки значения LogLevel.
        /// </summary>
        /// <returns></returns>
        public LogLevel LoadLogLevelFromConfig()
        {
            ConfigHelper confHelper = new ConfigHelper();
            string level = confHelper.GetConfigFromKey("LogLevel");
            LogLevel lvl;

            switch (level)
            {
                case "DEBUG":
                    {
                        lvl = LogLevel.DEBUG;
                        break;
                    }
                case "INFO":
                    {
                        lvl = LogLevel.INFO;
                        break;
                    }
                case "WARN":
                    {
                        lvl = LogLevel.WARN;
                        break;
                    }
                case "ERROR":
                    {
                        lvl = LogLevel.ERROR;
                        break;
                    }
                case "FATAL":
                    {
                        lvl = LogLevel.FATAL;
                        break;
                    }
                default:
                    {
                        lvl = LogLevel.DEBUG;
                        break;
                    }
            }
            return lvl;
        }

        /// <summary>
        /// Метод загрузки значения LogMode.
        /// </summary>
        /// <returns></returns>
        public LogMode LoadLogModeFromConfig()
        {
            ConfigHelper confHelper = new ConfigHelper();
            string mode = confHelper.GetConfigFromKey("LogMode");
            LogMode m;

            switch (mode)
            {
                case "SINGLE":
                    {
                        m = LogMode.SINGLE;
                        break;
                    }
                case "MULTIPLE":
                    {
                        m = LogMode.MULTIPLE;
                        break;
                    }
                default:
                    {
                        m = LogMode.MULTIPLE;
                        break;
                    }
            }
            return m;
        }

        /// <summary>
        /// Метод загрузки директории логирования.
        /// </summary>
        /// <returns>Директория логирования.</returns>
        public string LoadLogDirFromConfig()
        {
            ConfigHelper confHelper = new ConfigHelper();
            return confHelper.GetConfigFromKey("LogDir");
        }
        #endregion Read

        #region Write
        #endregion Write
    }
}

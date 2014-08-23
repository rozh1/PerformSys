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
using BalancerLogger.Helpers;
using BalancerLogger.Helpers.Config;
using BalancerLogger.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger
{
    /// <summary>
    /// Класс логировәика.
    /// </summary>
    public class BalancerLogger : IBalancerLogger
    {
        /// <summary>
        /// Писатель.
        /// </summary>
        private IWriter _writer = null;

        /// <summary>
        /// Уровень логирования.
        /// </summary>
        private LogLevel _logLevel;

        /// <summary>
        /// Тип логирования.
        /// </summary>
        private LogMode _logMode;

        /// <summary>
        /// Директория логирования.
        /// </summary>
        private string _logDir;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="concreteWriter">Конкретный тип писателя.</param>
        public BalancerLogger(IWriter concreteWriter)
        {
            _writer = concreteWriter;

            ReadConfig();
        }

        /// <summary>
        /// Метод для считывания и установки private свойств из конфигурационного файла BalancerLogger.dll.config
        /// </summary>
        private void ReadConfig()
        {
            ManageConfig mngCng = new ManageConfig();

            _logLevel = mngCng.LoadLogLevelFromConfig();
            _logMode = mngCng.LoadLogModeFromConfig();
            _logDir = mngCng.LoadLogDirFromConfig(); // TODO : по какой-то причине не считывает из конфига.

            if (String.IsNullOrEmpty(_logDir))
            {
                _logDir = "C:\\Users\\Diego\\Documents\\Visual Studio 2013\\Projects\\BalancerLogger\\BalancerLogger\\BalancerLogger\\bin\\Debug";
            }
        }

        /// <summary>
        /// Метод записи лога.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="concreteDataType">Писатель.</param>
        /// <param name="logLevel">Уровень логирования.</param>
        public void Log(string fileName, ILogData concreteDataType, LogLevel logLevel)
        {
            string filePath = CommonHelpers.GetAbsoluteFilePath(_logDir, fileName);
            CommonHelpers.CreateIfNotExistDirectory(filePath);

            if (logLevel >= _logLevel && _logMode == LogMode.MULTIPLE)
            {
                _writer.Write(filePath, concreteDataType.DataParams);
            }

            if (logLevel == _logLevel && _logMode == LogMode.SINGLE)
            {
                _writer.Write(filePath, concreteDataType.DataParams);
            }
        }
    }
}

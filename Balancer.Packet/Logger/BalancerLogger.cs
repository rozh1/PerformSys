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

﻿using Balancer.Common.Logger.Interfaces;
using BalancerLogger.Enums;
using BalancerLogger.Helpers;
using BalancerLogger.Interfaces;
using BalancerLogger.Writers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace BalancerLogger
{
    /// <summary>
    /// Класс логировәика.
    /// </summary>
    public static class BalancerLogger
    {
        /// <summary>
        /// Писатель.
        /// </summary>
        private static IWriter _writer = null;

        /// <summary>
        /// Уровень логирования.
        /// </summary>
        private static LogLevel _logLevel { get; set; }

        /// <summary>
        /// Тип логирования.
        /// </summary>
        private static LogMode _logMode { get; set; }

        /// <summary>
        /// Директория логирования.
        /// </summary>
        private static string _logDir { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="concreteWriter">Конкретный тип писателя.</param>
        static BalancerLogger()
        {
            _writer = new Writer();

            _logLevel = LogLevel.DEBUG;
            _logMode = LogMode.MULTIPLE;
            _logDir = Environment.CurrentDirectory;
        }

        /// <summary>
        /// Метод записи лога.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="concreteDataType">Писатель.</param>
        /// <param name="logLevel">Уровень логирования.</param>
        public static void Log(string fileName, ILogData concreteDataType, LogLevel logLevel)
        {

            string filePath = CommonHelpers.GetAbsoluteFilePath(_logDir, fileName);
            CommonHelpers.CreateIfNotExistDirectory(filePath);

            if(concreteDataType is ICsvLogData)
            {
                if (!File.Exists(filePath))
                {
                    _writer.Write(filePath, ((ICsvLogData)concreteDataType).DataColumnNames);                   
                }
            }

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

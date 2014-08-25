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
using Balancer.Common.Logger.Enums;
using Balancer.Common.Logger.Helpers;
using Balancer.Common.Logger.Interfaces;

namespace Balancer.Common.Logger
{
    /// <summary>
    ///     Класс логировщика.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        ///     Писатель.
        /// </summary>
        private static readonly IWriter _writer;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        static Logger()
        {
            _writer = new Writer();

            LogLevel = LogLevel.DEBUG;
            LogMode = LogMode.MULTIPLE;
            LogDir = Environment.CurrentDirectory;
            WriteToConsole = true;
        }

        /// <summary>
        ///     Уровень логирования.
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        /// <summary>
        ///     Тип логирования.
        /// </summary>
        public static LogMode LogMode { get; set; }

        /// <summary>
        ///     Директория логирования.
        /// </summary>
        public static string LogDir { get; set; }

        /// <summary>
        ///     Выводить в консоль
        /// </summary>
        public static bool WriteToConsole { get; set; }

        public static void Configure(ILogConfig config)
        {
            LogLevel = config.LogLevel;
            LogMode = config.LogMode;
            LogDir = string.IsNullOrEmpty(config.LogDir) ? Environment.CurrentDirectory : config.LogDir;
            WriteToConsole = config.WriteToConsole;
        }

        /// <summary>
        ///     Метод записи лога.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="concreteDataType">Писатель.</param>
        /// <param name="logLevel">Уровень логирования.</param>
        public static void Write(string fileName, ILogData concreteDataType, LogLevel logLevel)
        {
            bool needWriteToConsole = WriteToConsole;
            string filePath = CommonHelpers.GetAbsoluteFilePath(LogDir, fileName);
            CommonHelpers.CreateIfNotExistDirectory(filePath);

            if (concreteDataType is ICsvLogData)
            {
                if (!File.Exists(filePath))
                {
                    _writer.Write(filePath, ((ICsvLogData) concreteDataType).DataColumnNames);
                }
                needWriteToConsole = false;
            }

            if (needWriteToConsole) Console.WriteLine(concreteDataType.DataParams[0]);

            if (logLevel >= LogLevel && LogMode == LogMode.MULTIPLE)
            {
                _writer.Write(filePath, concreteDataType.DataParams);
            }

            if (logLevel == LogLevel && LogMode == LogMode.SINGLE)
            {
                _writer.Write(filePath, concreteDataType.DataParams);
            }
        }
    }
}
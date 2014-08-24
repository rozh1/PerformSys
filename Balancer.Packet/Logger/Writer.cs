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

﻿using Balancer.Common.Logger.Enums;
using Balancer.Common.Logger.Helpers;
using Balancer.Common.Logger.Helpers.WriterHelpers;
using Balancer.Common.Logger.Interfaces;

namespace Balancer.Common.Logger
{
    /// <summary>
    ///     Базовый класс писателя.
    /// </summary>
    public class Writer : IWriter
    {
        /// <summary>
        ///     Тип файла.
        /// </summary>
        private FileType _fileType;

        /// <summary>
        ///     Метод записи лога.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        public virtual void Write(string filePath, string[] data)
        {
            _fileType = CommonHelpers.DefineTypeFromPath(filePath);

            switch (_fileType)
            {
                case FileType.TXT:
                {
                    WriteTxt(filePath, data);
                    break;
                }
                case FileType.CSV:
                {
                    WriteCsv(filePath, data);
                    break;
                }
            }
        }

        /// <summary>
        ///     Метод записи лога в txt-файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        private void WriteTxt(string filePath, string[] data)
        {
            TxtWriterHelper.TxtWrite(filePath, data);
        }

        /// <summary>
        ///     Метод записи лога в csv-файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        private void WriteCsv(string filePath, string[] data)
        {
            CsvWriterHelper.CsvWrite(filePath, data);
        }
    }
}
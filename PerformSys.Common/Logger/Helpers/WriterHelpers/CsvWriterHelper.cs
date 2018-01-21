#region Copyright
/*
 * Copyright 2013-2018 Lenar Khisamiev, Roman Klassen
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
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace PerformSys.Common.Logger.Helpers.WriterHelpers
{
    /// <summary>
    /// </summary>
    public static class CsvWriterHelper
    {
        /// <summary>
        ///     Мьютекс для синхронизации записи лога в файл.
        /// </summary>
        private static readonly Mutex Mut = new Mutex();

        /// <summary>
        ///     Метод записи лога в .csv файл
        /// </summary>
        /// <param name="filePath">
        ///     Путь к .csv файлу.param>
        ///     <param name="data">Данные лога.</param>
        public static void CsvWrite(string filePath, string[] data)
        {
            Mut.WaitOne();

            string csvLine = string.Join(";", data);
            File.AppendAllText(filePath, csvLine + Environment.NewLine, Encoding.UTF8);

            Mut.ReleaseMutex();
        }
    }
}
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

namespace Balancer.Common.Logger.Helpers
{
    public static class CommonHelpers
    {
        /// <summary>
        ///     Метод, определяющий FileType по filePath.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns>FileType файла.</returns>
        public static FileType DefineTypeFromPath(string filePath)
        {
            int index = filePath.LastIndexOf('.');
            string exe = filePath.Substring(index + 1, filePath.Length - (index + 1));

            FileType type;

            switch (exe.ToUpper())
            {
                case "TXT":
                {
                    type = FileType.TXT;
                    break;
                }
                case "CSV":
                {
                    type = FileType.CSV;
                    break;
                }
                default:
                {
                    type = FileType.TXT;
                    break;
                }
            }
            return type;
        }

        /// <summary>
        ///     Метод создания директории файла, если директории не суәествует
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public static void CreateIfNotExistDirectory(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
        }

        /// <summary>
        ///     Метод получения абсолютного пути к файлу.
        /// </summary>
        /// <param name="dir">Директория.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Абсолютный путь к файлу.</returns>
        public static string GetAbsoluteFilePath(string dir, string fileName)
        {
            if (!String.IsNullOrEmpty(dir) && !String.IsNullOrEmpty(fileName))
            {
                if (!dir.EndsWith("\\"))
                {
                    dir = dir + "\\";
                }

                return dir + fileName;
            }
            return null;
        }
    }
}
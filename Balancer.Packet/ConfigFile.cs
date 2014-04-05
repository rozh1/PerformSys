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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Balancer.Common
{
    public class ConfigFile
    {
        private static string _configFilePath = "server.conf";
        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>();

        /// <summary>
        ///     Задает путь к файлу конфигурации
        /// </summary>
        /// <param name="path">путь к файлу</param>
        public static void SetConfigPath(string path)
        {
            _configFilePath = path;
        }

        /// <summary>
        ///     Загрузка конфигурации из файла
        /// </summary>
        /// <returns>Текст конфигурации</returns>
        public static string LoadSettings()
        {
            string conf = "";
            try
            {
                var fsread = new FileStream(_configFilePath, FileMode.Open, FileAccess.Read);
                if (fsread.Length < 1)
                {
                    fsread.Close();
                    return "";
                }

                var strR = new StreamReader(fsread); //Открываем файл на чтение

                //StreamReader strR = new StreamReader(ConfigFilePath); //Открываем файл на чтение

                var reg = new Regex("(.+?)( )*=( )*(.+?)( )*;");
                while (!strR.EndOfStream) //Читаем построчно
                {
                    String line = strR.ReadLine();
                    conf += line + "\n";
                    if (line != null && reg.IsMatch(line))
                    {
                        Match m = reg.Match(line);
                        Config.Add(m.Groups[1].Value, m.Groups[4].Value);
                    }
                }
                strR.Close();
                fsread.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            return conf;
        }

        /// <summary>
        ///     Получение значения по ключю из конфигурации
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns>значение</returns>
        public static string GetConfigValue(string key)
        {
            if (Config.ContainsKey(key))
            {
                return Config[key];
            }
            return null;
        }

        /// <summary>
        ///     Устанавливает значение ключа конфига
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">значение</param>
        public static void SetConfigValue(string key, string value)
        {
            if (Config.ContainsKey(key))
            {
                Config[key] = value;
            }
            else
            {
                Config.Add(key, value);
            }
        }

        /// <summary>
        ///     Устанавливает значение ключа конфига
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="value">значение</param>
        public static void SetConfigValue(string key, int value)
        {
            string val = value.ToString(CultureInfo.InvariantCulture);
            if (Config.ContainsKey(key))
            {
                Config[key] = val;
            }
            else
            {
                Config.Add(key, val);
            }
        }

        /// <summary>
        ///     Запись конфигурации в файл
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                var fs = new FileStream(_configFilePath, FileMode.Truncate, FileAccess.Write);
                var strW = new StreamWriter(fs);

                //StreamWriter strW = new StreamWriter(ConfigFilePath,false);

                foreach (var pair in Config)
                {
                    strW.WriteLine(pair.Key + "=" + pair.Value + ";");
                }

                strW.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        ///     Запись конфигурации в файл
        /// </summary>
        public static void SaveSettings(string conf)
        {
            try
            {
                var fs = new FileStream(_configFilePath, FileMode.Create, FileAccess.Write);
                var strW = new StreamWriter(fs);
                
                strW.Write(conf);

                strW.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
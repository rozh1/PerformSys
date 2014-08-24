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

﻿using BalancerLogger.Interfaces;

namespace BalancerLogger.LogData
{
    /// <summary>
    /// Класс txt-данных.
    /// </summary>
    public class TxtData : ILogData
    {
        /// <summary>
        /// Данные.
        /// </summary>
        public string[] DataParams { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="Data"></param>
        public TxtData(string Line)
        {
            DataParams = new[] { Line };
        }
    }
}

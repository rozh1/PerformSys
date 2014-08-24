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
using System.Globalization;
using Balancer.Common.Logger.Interfaces;

namespace client.Config.Data
{
    public class LogStats : ICsvLogData
    {
        public int QueryNumber { get; set; }
        public int ClientNumber { get; set; }
        public int ClientQueryNumber { get; set; }
        public TimeSpan QueryTime { get; set; }

        public string[] GetCsvParams()
        {
            return new[]
            {
                QueryNumber.ToString(CultureInfo.CurrentCulture),
                ClientNumber.ToString(CultureInfo.CurrentCulture),
                ClientQueryNumber.ToString(CultureInfo.CurrentCulture),
                QueryTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)
            };
        }

        public string[] GetCsvColumnNames()
        {
            return new[]
            {
                @"Номер запроса",
                @"Номер клиента",
                @"Номер запроса клиента",
                @"Время работы",
            };
        }
    }
}
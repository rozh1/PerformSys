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
using Balancer.Common.Logger.Data;

namespace rbn.Config.Data
{
    public class LogStats : ILogStats
    {
        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public TimeSpan QueryExecutionTime { get; set; }
        public TimeSpan QueueWaitTime { get; set; }
        public int QueueLength { get; set; }

        public string[] GetCsvParams()
        {
            return new[]
            {
                GlobalId.ToString(CultureInfo.InvariantCulture),
                RegionId.ToString(CultureInfo.InvariantCulture),
                ClientNumber.ToString(CultureInfo.InvariantCulture),
                QueryNumber.ToString(CultureInfo.InvariantCulture),
                QueryExecutionTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                QueueWaitTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                QueueLength.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}

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

﻿using System.Globalization;
using Balancer.Common.Logger.Interfaces;

namespace router.Config.Data.LogData
{
    public class QueueStats : ICsvLogData
    {
        private readonly string[] _columnNmaes =
        {
            @"Глобальный идентификатор",
            @"Номер региона",
            @"Номер клиента",
            @"Длина очереди"
        };

        public QueueStats(
            uint globalId,
            uint regionId,
            int clientNumber,
            int queueLength)
        {
            GlobalId = globalId;
            RegionId = regionId;
            ClientNumber = clientNumber;
            QueueLength = queueLength;
        }

        private uint GlobalId { get; set; }
        private uint RegionId { get; set; }
        private int ClientNumber { get; set; }
        private int QueueLength { get; set; }


        public string[] DataParams
        {
            get
            {
                return new[]
                {
                    GlobalId.ToString(CultureInfo.CurrentCulture),
                    RegionId.ToString(CultureInfo.CurrentCulture),
                    ClientNumber.ToString(CultureInfo.CurrentCulture),
                    QueueLength.ToString(CultureInfo.CurrentCulture)
                };
            }
        }

        public string[] DataColumnNames
        {
            get { return _columnNmaes; }
        }
    }
}
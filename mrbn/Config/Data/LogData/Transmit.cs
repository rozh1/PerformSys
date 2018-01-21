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
using PerformSys.Common.Logger.Interfaces;

namespace mrbn.Config.Data.LogData
{
    public class Transmit : ICsvLogData
    {
        public Transmit(
            uint globalId,
            uint regionId,
            int clientNumber,
            int queryNumber,
            double queueWeight,
            uint toGlobalId,
            uint toRegionId,
            double toQueueWeight)
        {
            GlobalId = globalId;
            RegionId = regionId;
            ClientNumber = clientNumber;
            QueryNumber = queryNumber;
            QueueWeight = queueWeight;
            ToGlobalId = toGlobalId;
            ToRegionId = toRegionId;
            ToQueueWeight = toQueueWeight;
        }

        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public double QueueWeight { get; set; }
        public uint ToGlobalId { get; set; }
        public uint ToRegionId { get; set; }
        public double ToQueueWeight { get; set; }

        public string[] DataParams
        {
            get
            {
                return new[]
                {
                    ClientNumber.ToString(CultureInfo.CurrentCulture),
                    QueryNumber.ToString(CultureInfo.CurrentCulture),
                    GlobalId.ToString(CultureInfo.CurrentCulture),
                    RegionId.ToString(CultureInfo.CurrentCulture),
                    QueueWeight.ToString(CultureInfo.CurrentCulture),
                    ToGlobalId.ToString(CultureInfo.CurrentCulture),
                    ToRegionId.ToString(CultureInfo.CurrentCulture),
                    ToQueueWeight.ToString(CultureInfo.CurrentCulture)
                };
            }
        }

        public string[] DataColumnNames
        {
            get
            {
                return new[]
                {
                    @"Номер клиента",
                    @"Номер запроса",
                    @"Глобальный идентификатор",
                    @"Номер региона",
                    @"Вес очереди очереди",
                    @"Глобальный идентификатор",
                    @"Номер региона",
                    @"Вес очереди очереди",
                };
            }
        }
    }
}
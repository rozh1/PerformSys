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
using System.Net.Sockets;
using PerformSys.Common.Utils.Interfaces;

namespace mrbn.GlobalBalancer.Data
{
    internal class RBN : ICloneable<RBN>
    {
        private double _weight;
        private RBN _relayRbn;

        public TcpClient RbnClient { get; set; }
        public uint RegionId { get; set; }
        public uint GlobalId { get; set; }

        public double Weight
        {
            get { return _weight; }
            set
            {
                if (Math.Abs(_weight - value) > 0.001)
                {
                    _weight = value;
                    if (WeightChanged != null)
                    {
                        WeightChanged();
                    }
                    if (RelayRbn != null && (Weight - RelayRbn.Weight) > 0.001)
                    {
                        TransmitRequest();
                    }
                }
            }
        }

        public RBN RelayRbn
        {
            get { return _relayRbn; }
            set
            {
                if (_relayRbn != value)
                {
                    _relayRbn = value;
                    if (RelayRbnChanged != null)
                    {
                        RelayRbnChanged();
                    }
                }
            }
        }

        public event Action WeightChanged;
        public event Action RelayRbnChanged;
        public event Action TransmitRequest;

        public RBN Clone()
        {
            var ret = new RBN
            {
                Weight = Weight,
                GlobalId = GlobalId,
                RegionId = RegionId,
                RbnClient = RbnClient
            };
            return ret;
        }
    }
}
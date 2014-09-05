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
using Balancer.Common.Utils.Interfaces;

namespace mrbn.GlobalBalancer.Data
{
    internal class RBN : ICloneable<RBN>
    {
        private double _weight;

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
                }
            }
        }

        public RBN RelayRbn { get; set; }

        public event Action WeightChanged;

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
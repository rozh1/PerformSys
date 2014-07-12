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
using System.Linq;
using mrbn.GlobalBalancer.Data;

namespace mrbn.GlobalBalancer
{
    class GlobalBalancer
    {
        private readonly List<RBN> _rbns;

        private readonly object _rbnsSyncObject;

        public GlobalBalancer()
        {
            _rbnsSyncObject = new object();
            _rbns = new List<RBN>();
        }

        public bool AddRbn(RBN rbn)
        {
            bool result = false;

            lock (_rbnsSyncObject)
            {
                RBN existRbn = _rbns.FirstOrDefault(curRbn => curRbn.RegionId == rbn.RegionId);
                if (existRbn == null)
                {
                    _rbns.Add(rbn);
                    result = true;
                }
            }

            return result;
        }

        public void Remove(RBN rbn)
        {
            lock (_rbnsSyncObject)
            {
                if (_rbns.Contains(rbn))
                {
                    _rbns.Remove(rbn);
                }
            }
        }

        public void ConnectRbns()
        {
            RBN lowLoadRbn;
            RBN highLoadRbn = lowLoadRbn = new RBN { RbnClient = null, RegionId = 0, Weight = 0 };

            lock (_rbnsSyncObject)
            {
                foreach (var rbn in _rbns)
                {
                    if (rbn.Weight > highLoadRbn.Weight && rbn.RelayRbn == null) highLoadRbn = rbn;
                    if (rbn.Weight < lowLoadRbn.Weight) lowLoadRbn = rbn;
                }

                if (highLoadRbn != lowLoadRbn)
                {
                    if (highLoadRbn.RegionId > 0)
                    {
                        highLoadRbn.RelayRbn = lowLoadRbn;
                    }
                }
            }
        }

        public RBN GetRbnByRegionId(int id)
        {
            return _rbns.FirstOrDefault(curRbn => curRbn.RegionId == id);
        }

        public RBN[] GetAllRBNs()
        {
            RBN[] rbns;

            lock (_rbnsSyncObject)
            {
                rbns = _rbns.ToArray();
            }

            return rbns;
        }

        public Dictionary<string, UInt64>[] GetAllTableSizes()
        {
            var tableSizes = new Dictionary<string, ulong>[_rbns.Count];
            for (int i = 0; i < _rbns.Count; i++)
            {
                tableSizes[i] = _rbns[i].TableSizes;
            }
            return tableSizes;
        }
    }
}

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
using client.Config;
using client.Config.Data;

namespace client.QuerySequence
{
    internal class QuerySequenceManager
    {
        private readonly ClientConfig _config;

        public QuerySequenceManager(ClientConfig config)
        {
            _config = config;
        }

        public IQuerySequence GetQuerySequence(int startQueryNumber, int clientNumber)
        {
            switch (_config.QuerySequence.Mode)
            {
                case QuerySequenceMode.Sequential:
                    return new SequentialQuerySequence(14, startQueryNumber);
                case QuerySequenceMode.Random:
                    return new RandomSequentialQuerySequence(14);
                case QuerySequenceMode.FromList:
                    return new ListQuerySequence(_config.QuerySequence.List, clientNumber, _config.Scenario.ClientCount);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
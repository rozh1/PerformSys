#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen, Lenar Khisamiev
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
using System;
using client.Config;
using client.Config.Data;

namespace client.QuerySequence
{
    internal class QuerySequenceManager
    {
        private readonly ClientConfig _config;
        private readonly ListQuerySequence _listQuerySequence;

        public QuerySequenceManager(ClientConfig config)
        {
            _config = config;
            _listQuerySequence = new ListQuerySequence(_config.QuerySequence.List);
        }

        public IQuerySequence GetQuerySequence(int startQueryNumber, int queriesCount)
        {
            switch (_config.QuerySequence.Mode)
            {
                case QuerySequenceMode.Sequential:
                    return new SequentialQuerySequence(queriesCount, startQueryNumber);
                case QuerySequenceMode.Random:
                    return new RandomSequentialQuerySequence(queriesCount);
                case QuerySequenceMode.FromList:
                    return _listQuerySequence;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
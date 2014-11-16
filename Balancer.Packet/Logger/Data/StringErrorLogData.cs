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

﻿using System.Collections.Generic;
using Balancer.Common.Logger.Interfaces;

namespace Balancer.Common.Logger.Data
{
    public class StringErorrLogData : ILogData
    {
        public StringErorrLogData(string stringFormatPattern, string methodName, params string[] parameters)
        {
            var strings = new List<object> {methodName};
            strings.AddRange(parameters);

            DataParams = new[] { string.Format(stringFormatPattern, strings.ToArray()) };
        }

        public string[] DataParams { get; set; }
    }
}
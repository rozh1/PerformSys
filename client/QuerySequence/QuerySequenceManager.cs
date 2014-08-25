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
using System.Text;

namespace client.QuerySequence
{
    /// <summary>
    /// Класс для генерации случайной последовательности следования номеров запросов.
    /// </summary>
    public static class QuerySequenceManager
    {
        /// <summary>
        /// Метод для генерации случайной последовательности следования номеров запросов.
        /// </summary>
        /// <param name="maxQueryCount">Максимальный номер запроса.</param>
        /// <returns>Случайная последовательность следования номеров запросов.</returns>
        public static QuerySequence Generate(int maxQueryCount)
        {
            int[] array = Enumerable.Range(1, maxQueryCount).ToArray();
            var random = new Random(DateTime.Now.Millisecond);
            array = array.OrderBy(x => random.Next()).ToArray();

            return new QuerySequence() { array = array };
        }
    }
}

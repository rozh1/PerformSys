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

namespace client.QuerySequence
{
    /// <summary>
    ///     Контракт последовательности запросов.
    /// </summary>
    public class RandomSequentialQuerySequence : QuerySequenceBase, IQuerySequence
    {
        /// <summary> количество запросов </summary>
        private readonly int _queryCount;

        /// <summary> Великий и могучий рандом </summary>
        private readonly Random _random;

        /// <summary>
        ///     Конструктор последовательности
        /// </summary>
        /// <param name="queryCount">Количество запросов</param>
        public RandomSequentialQuerySequence(int queryCount)
        {
            _queryCount = queryCount;
            _random = new Random(DateTime.UtcNow.Millisecond);
        }

        /// <summary>
        ///     Получает номер следующий номер запроса
        /// </summary>
        /// <returns>номер запроса</returns>
        public int GetNextQueryNumber()
        {
            int nextNumber;
            lock (GetNextQueryLockObject)
            {
                nextNumber = _random.Next(1, _queryCount);
            }
            return nextNumber;
        }

        /// <summary>
        ///     Проверяет возможность выдачи следующего номера запроса
        /// </summary>
        /// <returns></returns>
        public bool CanGetNextQueryNumber()
        {
            return true;
        }
    }
}
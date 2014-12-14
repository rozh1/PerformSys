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
using client.Config.Data;

namespace client.QuerySequence
{
    /// <summary>
    ///     Контракт последовательности запросов.
    /// </summary>
    public class ListQuerySequence : QuerySequenceBase, IQuerySequence
    {
        /// <summary> Массив c последовательностью номеров запросов. </summary>
        private readonly int[] _array;

        /// <summary> Индекс текущего элмента в массиве </summary>
        private int _queryIndex;

        /// <summary>
        ///     Конструктор последовательности
        /// </summary>
        /// <param name="queryConfigList">Список запросов</param>
        /// <param name="clientNumber">номер клиента</param>
        /// <param name="clientCount">количество клиентов</param>
        public ListQuerySequence(QueryConfig[] queryConfigList, int clientNumber, int clientCount)
        {
            var queries = new List<int>();
            for (int i = clientNumber; i < queryConfigList.Length; i += clientCount)
            {
                queries.Add(queryConfigList[i].Number);
            }
            _array = queries.ToArray();
        }

        /// <summary>
        ///     Получает номер следующий номер запроса
        /// </summary>
        /// <returns>номер запроса</returns>
        public int GetNextQueryNumber()
        {
            int nextNumber = 0;
            lock (GetNextQueryLockObject)
            {
                if (_array.Length > _queryIndex)
                    nextNumber = _array[_queryIndex++];
            }
            return nextNumber;
        }

        /// <summary>
        ///     Проверяет возможность выдачи следующего номера запроса
        /// </summary>
        /// <returns></returns>
        public bool CanGetNextQueryNumber()
        {
            bool canGet;
            lock (GetNextQueryLockObject)
            {
                canGet = _array.Length > _queryIndex;
            }
            return canGet;
        }
    }
}
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

﻿namespace client.QuerySequence
{
    /// <summary>
    ///     Контракт последовательности запросов.
    /// </summary>
    public class SequentialQuerySequence : QuerySequenceBase, IQuerySequence
    {
        /// <summary> предыдущий индекс в массиве </summary>
        private int _arrayLastIndex;

        /// <summary> номер следующего запроса </summary>
        private int _nextQueryNumber;

        /// <summary>
        ///     Конструктор последовательности
        /// </summary>
        /// <param name="startQueryNumber">Номер запроса с которого начинается выдача запросов</param>
        /// <param name="queryCount">Количество запросов</param>
        public SequentialQuerySequence(int queryCount, int startQueryNumber = 1)
        {
            Array = GenerateLine(queryCount);
            _nextQueryNumber = startQueryNumber;
            _arrayLastIndex = FindIndexByQueryNumber(_nextQueryNumber);
        }

        /// <summary>
        ///     Массив c последовательностью номеров запросов.
        /// </summary>
        private int[] Array { get; set; }

        /// <summary>
        ///     Получает номер следующий номер запроса
        /// </summary>
        /// <returns>номер запроса</returns>
        public int GetNextQueryNumber()
        {
            int queryNumber;
            lock (GetNextQueryLockObject)
            {
                queryNumber = _nextQueryNumber;
                _nextQueryNumber = Array[_arrayLastIndex];
                _arrayLastIndex = GetNextIndex(_arrayLastIndex, Array.Length);
            }
            return queryNumber;
        }

        /// <summary>
        ///     Проверяет возможность выдачи следующего номера запроса
        /// </summary>
        /// <returns></returns>
        public bool CanGetNextQueryNumber()
        {
            return true;
        }

        /// <summary>
        ///     Метод для генерации линейной последовательности следования номеров запросов.
        /// </summary>
        /// <param name="maxQueryCount">Максимальный номер запроса.</param>
        /// <returns>Линейная последовательность следования номеров запросов.</returns>
        private int[] GenerateLine(int maxQueryCount)
        {
            var array = new int[maxQueryCount];

            for (int i = 0; i < maxQueryCount; i++)
            {
                array[i] = (i + 1);
            }

            return array;
        }

        /// <summary>
        ///     Находит номер индекса в массиве по номеру запроса
        /// </summary>
        /// <param name="queryNumber">Номер запроса</param>
        /// <returns>Номер индекса</returns>
        private int FindIndexByQueryNumber(int queryNumber)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] == queryNumber)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        ///     Получает номер следующего элемента в массиве
        /// </summary>
        /// <param name="lastIndex">последний номер</param>
        /// <param name="arrayLength">длина массива</param>
        /// <returns>новый индекс</returns>
        private int GetNextIndex(int lastIndex, int arrayLength)
        {
            int newIndex;
            int lenght = arrayLength - 1;
            if (lastIndex < lenght)
            {
                newIndex = lastIndex + 1;
            }
            else
            {
                newIndex = 0;
            }
            return newIndex;
        }
    }
}
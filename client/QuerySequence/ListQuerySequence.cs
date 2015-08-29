using System.Collections.Generic;
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
        /// <param name="queryNumbers">Список номеров запросов</param>
        public ListQuerySequence(int[] queryNumbers)
        {
            var queries = new List<int>();
            for (int i = 0; i < queryNumbers.Length; i++)
            {
                queries.Add(queryNumbers[i]);
            }
            _array = queries.ToArray();
        }

        /// <summary>
        ///     Получает номер следующий номер запроса
        /// </summary>
        /// <returns>номер запроса или 0, если запросы закончились</returns>
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
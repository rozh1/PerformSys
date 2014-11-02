using System.Collections.Generic;
using client.Config.Data;

namespace client.QuerySequence
{
    /// <summary>
    ///     Контракт последовательности запросов.
    /// </summary>
    public class ListQuerySequence : IQuerySequence
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
            if (_array.Length > _queryIndex)
                return _array[_queryIndex++];
            return 0;
        }

        /// <summary>
        ///     Проверяет возможность выдачи следующего номера запроса
        /// </summary>
        /// <returns></returns>
        public bool CanGetNextQueryNumber()
        {
            return _array.Length > _queryIndex;
        }
    }
}
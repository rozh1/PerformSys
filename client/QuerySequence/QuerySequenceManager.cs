using System;
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

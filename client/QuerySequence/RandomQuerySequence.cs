using System;

namespace client.QuerySequence
{
    /// <summary>
    ///     Контракт последовательности запросов.
    /// </summary>
    public class RandomSequentialQuerySequence : IQuerySequence
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
            return _random.Next(1, _queryCount);
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
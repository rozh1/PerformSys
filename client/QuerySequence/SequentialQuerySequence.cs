namespace client.QuerySequence
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
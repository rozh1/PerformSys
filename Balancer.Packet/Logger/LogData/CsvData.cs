using BalancerLogger.Interfaces;

namespace BalancerLogger.LogData
{
    /// <summary>
    /// Класс csv-данных.
    /// </summary>
    public class CsvData : ILogData
    {
        /// <summary>
        /// Данные.
        /// </summary>
        public string[] DataParams { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="Data"></param>
        public CsvData(string[] Data)
        {
            DataParams = Data;
        }
    }
}

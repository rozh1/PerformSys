using BalancerLogger.Interfaces;

namespace BalancerLogger.LogData
{
    /// <summary>
    /// Класс txt-данных.
    /// </summary>
    public class TxtData : ILogData
    {
        /// <summary>
        /// Данные.
        /// </summary>
        public string[] DataParams { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="Data"></param>
        public TxtData(string Line)
        {
            DataParams = new[] { Line };
        }
    }
}

using BalancerLogger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

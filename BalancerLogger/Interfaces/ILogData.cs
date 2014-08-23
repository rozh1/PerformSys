using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Interfaces
{
    /// <summary>
    /// Интерфейс формата данных для логировщика.
    /// </summary>
    public interface ILogData
    {
        string[] DataParams { get; set; }
    }
}

using BalancerLogger.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Interfaces
{
    /// <summary>
    /// Интерфейс логировщика.
    /// </summary>
    interface IBalancerLogger
    {
        void Log(string fileName, ILogData concreteDataType, LogLevel logLevel);
    }
}

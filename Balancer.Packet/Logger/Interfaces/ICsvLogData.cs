using BalancerLogger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balancer.Common.Logger.Interfaces
{       
    /// <summary>
    /// Интерфейс формата данных логировщика .
    /// </summary>
    public interface ICsvLogData : ILogData
    {
        string[] DataColumnNames { get; set; }
        string[] DataParams { get; set; }
    }
}

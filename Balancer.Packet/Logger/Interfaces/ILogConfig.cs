using Balancer.Common.Logger.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balancer.Common.Logger.Interfaces
{
    public interface ILogConfig
    {
        LogLevel LogLevel { get; set; }
        LogMode LogMode { get; set; }
        string LogDir { get; set; }
        bool WriteToConsole { get; set; }
    }
}

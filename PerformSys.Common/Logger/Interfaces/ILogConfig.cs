using PerformSys.Common.Logger.Enums;

namespace PerformSys.Common.Logger.Interfaces
{
    public interface ILogConfig
    {
        LogLevel LogLevel { get; set; }
        LogMode LogMode { get; set; }
        string LogDir { get; set; }
        bool WriteToConsole { get; set; }
    }
}

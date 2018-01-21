using System;
using System.Globalization;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Interfaces;

namespace rbn.Config.Data
{
    public class LogStats : ICsvLogData
    {
        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public TimeSpan QueryExecutionTime { get; set; }
        public TimeSpan QueueWaitTime { get; set; }
        public int QueueLength { get; set; }

        public string[] DataParams
        {
            get
            {
                return new[]
                {
                    GlobalId.ToString(CultureInfo.CurrentCulture),
                    RegionId.ToString(CultureInfo.CurrentCulture),
                    ClientNumber.ToString(CultureInfo.CurrentCulture),
                    QueryNumber.ToString(CultureInfo.CurrentCulture),
                    QueryExecutionTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture),
                    QueueWaitTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture),
                    QueueLength.ToString(CultureInfo.CurrentCulture),
                };
            }
        }

        public string[] DataColumnNames
        {
            get
            {
                return new[]
                {
                    @"Глобальный идентификатор",
                    @"Номер региона",
                    @"Номер клиента",
                    @"Номер запроса",
                    @"Время выполнения",
                    @"Время ожидания",
                    @"Длина очереди",
                };
            }
        }
    }
}

using System;
using System.Globalization;
using Balancer.Common.Logger.Data;

namespace rbn.Config.Data
{
    public class LogStats : ILogStats
    {
        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public TimeSpan QueryExecutionTime { get; set; }
        public TimeSpan QueueWaitTime { get; set; }
        public int QueueLength { get; set; }

        public string[] GetCsvParams()
        {
            return new[]
            {
                GlobalId.ToString(CultureInfo.InvariantCulture),
                RegionId.ToString(CultureInfo.InvariantCulture),
                ClientNumber.ToString(CultureInfo.InvariantCulture),
                QueryNumber.ToString(CultureInfo.InvariantCulture),
                QueryExecutionTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                QueueWaitTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                QueueLength.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}

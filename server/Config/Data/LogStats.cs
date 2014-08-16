using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Balancer.Common.Logger.Data;

namespace server.Config.Data
{
    public class LogStats : ILogStats
    {
        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public TimeSpan QueryExecutionTime { get; set; }
        public int QueueLength { get; set; }

        public string[] GetCsvParams()
        {
            return new[]
            {
                GlobalId.ToString(CultureInfo.CurrentCulture),
                RegionId.ToString(CultureInfo.CurrentCulture),
                ClientNumber.ToString(CultureInfo.CurrentCulture),
                QueryNumber.ToString(CultureInfo.CurrentCulture),
                QueryExecutionTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture),
                QueueLength.ToString(CultureInfo.CurrentCulture),
            };
        }

        public string[] GetCsvColumnNames()
        {
            return new[]
            {
                @"Глобальный идентификатор",
                @"Номер региона",
                @"Номер клиента",
                @"Номер запроса",
                @"Время выполнения",
                @"Длина очереди",
            };
        }
    }
}

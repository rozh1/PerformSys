using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Interfaces;

namespace server.Config.Data
{
    public class LogStats : ICsvLogData
    {
        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public TimeSpan QueryExecutionTime { get; set; }
        public int QueueLength { get; set; }

        public string[] DataParams {
            get
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
                    @"Длина очереди",
                };
            }
        }
    }
}

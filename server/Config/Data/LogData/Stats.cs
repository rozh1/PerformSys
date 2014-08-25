using System;
using System.Globalization;
using Balancer.Common.Logger.Interfaces;

namespace server.Config.Data.LogData
{
    public class Stats : ICsvLogData
    {
        public Stats(
            uint globalId,
            uint regionId,
            int clientNumber,
            int queryNumber,
            TimeSpan queryExecutionTime,
            int queueLength)
        {
            GlobalId = globalId;
            RegionId = regionId;
            ClientNumber = clientNumber;
            QueryNumber = queryNumber;
            QueryExecutionTime = queryExecutionTime;
            QueueLength = queueLength;
        }

        private uint GlobalId { get; set; }
        private uint RegionId { get; set; }
        private int ClientNumber { get; set; }
        private int QueryNumber { get; set; }
        private TimeSpan QueryExecutionTime { get; set; }
        private int QueueLength { get; set; }

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
                    QueueLength.ToString(CultureInfo.CurrentCulture)
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
                    @"Длина очереди"
                };
            }
        }
    }
}
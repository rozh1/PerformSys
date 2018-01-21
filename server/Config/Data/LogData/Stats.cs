using System;
using System.Globalization;
using PerformSys.Common.Logger.Interfaces;

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
            long responseLenght,
            int queueLength)
        {
            GlobalId = globalId;
            RegionId = regionId;
            ClientNumber = clientNumber;
            QueryNumber = queryNumber;
            QueryExecutionTime = queryExecutionTime;
            ResponseLenght = responseLenght;
            QueueLength = queueLength;
        }

        private uint GlobalId { get; set; }
        private uint RegionId { get; set; }
        private int ClientNumber { get; set; }
        private int QueryNumber { get; set; }
        private TimeSpan QueryExecutionTime { get; set; }
        private long ResponseLenght { get; set; }
        private int QueueLength { get; set; }

        public string[] DataParams
        {
            get
            {
                return new[]
                {
                    DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    GlobalId.ToString(CultureInfo.CurrentCulture),
                    RegionId.ToString(CultureInfo.CurrentCulture),
                    ClientNumber.ToString(CultureInfo.CurrentCulture),
                    QueryNumber.ToString(CultureInfo.CurrentCulture),
                    QueryExecutionTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture),
                    ResponseLenght.ToString(CultureInfo.CurrentCulture),
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
                    @"Время",
                    @"Глобальный идентификатор",
                    @"Номер региона",
                    @"Номер клиента",
                    @"Номер запроса",
                    @"Время выполнения",
                    @"Длина ответа (byte)",
                    @"Длина очереди"
                };
            }
        }
    }
}
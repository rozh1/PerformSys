using System.Globalization;
using Balancer.Common.Logger.Interfaces;

namespace rbn.Config.Data.LogData
{
    public class QueueStats : ICsvLogData
    {
        public QueueStats(
            uint globalId,
            uint regionId,
            int clientNumber,
            int queueLength)
        {
            GlobalId = globalId;
            RegionId = regionId;
            ClientNumber = clientNumber;
            QueueLength = queueLength;
        }

        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
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
                    @"Длина очереди"
                };
            }
        }
    }
}
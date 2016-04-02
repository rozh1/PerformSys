using System.Globalization;
using Balancer.Common.Logger.Interfaces;

namespace router.Config.Data.LogData
{
    public class QueueStats : ICsvLogData
    {
        private readonly string[] _columnNmaes =
        {
            @"Глобальный идентификатор",
            @"Номер региона",
            @"Номер клиента",
            @"Длина очереди"
        };

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

        private uint GlobalId { get; set; }
        private uint RegionId { get; set; }
        private int ClientNumber { get; set; }
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
                    QueueLength.ToString(CultureInfo.CurrentCulture)
                };
            }
        }

        public string[] DataColumnNames
        {
            get { return _columnNmaes; }
        }
    }
}
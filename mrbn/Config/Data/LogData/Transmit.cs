using System.Globalization;
using PerformSys.Common.Logger.Interfaces;

namespace mrbn.Config.Data.LogData
{
    public class Transmit : ICsvLogData
    {
        public Transmit(
            uint globalId,
            uint regionId,
            int clientNumber,
            int queryNumber,
            double queueWeight,
            uint toGlobalId,
            uint toRegionId,
            double toQueueWeight)
        {
            GlobalId = globalId;
            RegionId = regionId;
            ClientNumber = clientNumber;
            QueryNumber = queryNumber;
            QueueWeight = queueWeight;
            ToGlobalId = toGlobalId;
            ToRegionId = toRegionId;
            ToQueueWeight = toQueueWeight;
        }

        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public int ClientNumber { get; set; }
        public int QueryNumber { get; set; }
        public double QueueWeight { get; set; }
        public uint ToGlobalId { get; set; }
        public uint ToRegionId { get; set; }
        public double ToQueueWeight { get; set; }

        public string[] DataParams
        {
            get
            {
                return new[]
                {
                    ClientNumber.ToString(CultureInfo.CurrentCulture),
                    QueryNumber.ToString(CultureInfo.CurrentCulture),
                    GlobalId.ToString(CultureInfo.CurrentCulture),
                    RegionId.ToString(CultureInfo.CurrentCulture),
                    QueueWeight.ToString(CultureInfo.CurrentCulture),
                    ToGlobalId.ToString(CultureInfo.CurrentCulture),
                    ToRegionId.ToString(CultureInfo.CurrentCulture),
                    ToQueueWeight.ToString(CultureInfo.CurrentCulture)
                };
            }
        }

        public string[] DataColumnNames
        {
            get
            {
                return new[]
                {
                    @"Номер клиента",
                    @"Номер запроса",
                    @"Глобальный идентификатор",
                    @"Номер региона",
                    @"Вес очереди очереди",
                    @"Глобальный идентификатор",
                    @"Номер региона",
                    @"Вес очереди очереди",
                };
            }
        }
    }
}
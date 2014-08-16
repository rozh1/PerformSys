using System;
using System.Globalization;
using Balancer.Common.Logger.Data;

namespace client.Config.Data
{
    public class LogStats : ILogStats
    {
        public int QueryNumber { get; set; }
        public int ClientNumber { get; set; }
        public int ClientQueryNumber { get; set; }
        public TimeSpan QueryTime { get; set; }

        public string[] GetCsvParams()
        {
            return new[]
            {
                QueryNumber.ToString(CultureInfo.CurrentCulture),
                ClientNumber.ToString(CultureInfo.CurrentCulture),
                ClientQueryNumber.ToString(CultureInfo.CurrentCulture),
                QueryTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)
            };
        }

        public string[] GetCsvColumnNames()
        {
            return new[]
            {
                @"Номер запроса",
                @"Номер клиента",
                @"Номер запроса клиента",
                @"Время работы",
            };
        }
    }
}
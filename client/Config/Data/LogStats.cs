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
                QueryNumber.ToString(CultureInfo.InvariantCulture),
                ClientNumber.ToString(CultureInfo.InvariantCulture),
                ClientQueryNumber.ToString(CultureInfo.InvariantCulture),
                QueryTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
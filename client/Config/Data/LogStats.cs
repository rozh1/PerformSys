using System;
using System.Globalization;
using PerformSys.Common.Logger.Interfaces;

namespace client.Config.Data
{
    public class LogStats : ICsvLogData
    {
        public int QueryNumber { get; set; }
        public int ClientNumber { get; set; }
        public int ClientQueryNumber { get; set; }
        public TimeSpan QueryTime { get; set; }

        public string[] DataColumnNames
        {
            get
            {
                return new[]
                {
                    @"Номер запроса",
                    @"Номер клиента",
                    @"Номер запроса клиента",
                    @"Время работы"
                };
            }
        }

        public string[] DataParams
        {
            get
            {
                return new[]
                {
                    QueryNumber.ToString(CultureInfo.CurrentCulture),
                    ClientNumber.ToString(CultureInfo.CurrentCulture),
                    ClientQueryNumber.ToString(CultureInfo.CurrentCulture),
                    QueryTime.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)
                };
            }
        }
    }
}
using System.Collections.Generic;
using PerformSys.Common.Logger.Interfaces;

namespace PerformSys.Common.Logger.Data
{
    public class StringErorrLogData : ILogData
    {
        public StringErorrLogData(string stringFormatPattern, string methodName, params string[] parameters)
        {
            var strings = new List<object> {methodName};
            strings.AddRange(parameters);

            DataParams = new[] { string.Format(stringFormatPattern, strings.ToArray()) };
        }

        public string[] DataParams { get; set; }
    }
}
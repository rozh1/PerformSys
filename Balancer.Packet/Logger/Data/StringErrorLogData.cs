using System.Collections.Generic;
using Balancer.Common.Logger.Interfaces;

namespace Balancer.Common.Logger.Data
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
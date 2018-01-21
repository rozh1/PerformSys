using System.Collections.Generic;
using client.Config.Data;
using PerformSys.Common.Config;

namespace client.Config
{
    public class QueriesList : ConfigBase<QueriesList>
    {
        /// <summary>
        ///     Список запросов
        /// </summary>
        public List<QueryConfig> Queries { get; set; }
    }
}
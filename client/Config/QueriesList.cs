using System.Collections.Generic;
using Balancer.Common.Config;
using client.Config.Data;

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
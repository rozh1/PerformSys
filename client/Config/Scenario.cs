using System;
using Balancer.Common.Config;
using client.Config.Data;

namespace client.Config
{
    public class Scenario : ConfigBase<Scenario>
    {
        /// <summary>
        ///     Количество клиентов
        /// </summary>
        public int ClientCount { get; set; }

        /// <summary>
        ///     Время начала выполнения сценария
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Последовательность действий сценария
        /// </summary>
        public ScenarioStep[] ScenarioSteps { get; set; }
    }
}
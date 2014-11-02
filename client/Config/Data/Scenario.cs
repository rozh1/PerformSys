using System;
using System.Xml.Serialization;

namespace client.Config.Data
{
    public class Scenario
    {
        /// <summary>
        ///     Количество клиентов
        /// </summary>
        [XmlAttribute]
        public int ClientCount { get; set; }

        /// <summary>
        ///     Время начала выполнения сценария
        /// </summary>
        [XmlAttribute]
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Последовательность действий сценария
        /// </summary>
        public ScenarioStep[] ScenarioSteps { get; set; }
    }
}
using Balancer.Common.Config;
using client.Config.Data;

namespace client.Config
{
    public class ClientConfig : ConfigBase<ClientConfig>
    {
        /// <summary>
        ///     Конфиг сервера
        /// </summary>
        public Server Server { get; set; }

        /// <summary>
        ///     Конфиг лога
        /// </summary>
        public Log Log { get; set; }

        public Data.QuerySequence QuerySequence { get; set; }

        /// <summary>
        ///     Сценарий работы клиентов
        /// </summary>
        public Scenario Scenario { get; set; }
    }
}
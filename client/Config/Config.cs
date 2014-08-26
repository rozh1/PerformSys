using client.Config.Data;

namespace client.Config
{
    public class Config
    {
        /// <summary>
        ///     Количество клиентов
        /// </summary>
        public int? ClientCount { get; set; }

        /// <summary>
        ///     Количество запросов на клиента
        /// </summary>
        public int? QueryCount { get; set; }

        /// <summary>
        ///     Адрес балансировщика
        /// </summary>
        public string BalancerHost { get; set; }

        /// <summary>
        ///     Порт балансировщика
        /// </summary>
        public int? BalancerPort { get; set; }

        /// <summary>
        /// Данные статистики для записи в лог
        /// </summary>
        public LogStats LogStats { get; set; }

        /// <summary>
        ///     Конфиг лога
        /// </summary>
        public Log Log { get; set; }
    }
}
namespace client.Config
{
    internal class Config
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
    }
}
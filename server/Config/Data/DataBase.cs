namespace server.Config.Data
{
    public class DataBase
    {
        public int RegionId { get; set; }
        public string Host { get; set; }
        public uint Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DataBaseName { get; set; }

        /// <summary>
        ///     Конфиг времен выполения запросов для режима симуляции
        /// </summary>
        public Data.SimulationParams SimulationParams { get; set; }

        /// <summary>
        ///     Конфиг размеров таблиц для симуляции
        /// </summary>
        public Data.SimulationSizes SimulationSizes { get; set; }
    }
}
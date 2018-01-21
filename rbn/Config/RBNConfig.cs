using PerformSys.Common.Config;
using rbn.Config.Data;

namespace rbn.Config
{
    public class RBNConfig : ConfigBase<RBNConfig>
    {
        /// <summary>
        ///     Конфиг сервера клиентов
        /// </summary>
        public Data.Server Server { get; set; }

        /// <summary>
        ///     Конфиг параметров РБН
        /// </summary>
        public Data.RBN RBN { get; set; }

        /// <summary>
        ///     Конфиг подключения к МРБН
        /// </summary>
        public Data.MRBN MRBN { get; set; }

        /// <summary>
        ///     Конфиг логировщика
        /// </summary>
        public Log Log { get; set; }
    }
}
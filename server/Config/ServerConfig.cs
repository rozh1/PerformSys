using Balancer.Common.Config;
using server.Config.Data;

namespace server.Config
{
    public class ServerConfig : ConfigBase<ServerConfig>
    {
        /// <summary>
        ///     Конфиг сервера
        /// </summary>
        public Data.Server Server { get; set; }

        /// <summary>
        ///     Конфиг базы данных
        /// </summary>
        public Data.DataBase[] DataBase { get; set; }

        /// <summary>
        ///     Конфиг лога
        /// </summary>
        public Log Log { get; set; }
    }
}
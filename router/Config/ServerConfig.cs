using PerformSys.Common.Config;
using router.Config.Data;

namespace router.Config
{
    public class RouterConfig : ConfigBase<RouterConfig>
    {
        /// <summary>
        ///     Конфиг сервера
        /// </summary>
        public Router Router { get; set; }
        
        /// <summary>
        ///     Конфиг лога
        /// </summary>
        public Log Log { get; set; }
    }
}
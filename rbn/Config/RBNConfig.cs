using System;
using System.IO;
using System.Xml.Serialization;
using Balancer.Common.Config;
using rbn.Config.Data;

namespace rbn.Config
{
    public class RBNConfig : ConfigBase<RBNConfig>
    {
        public Data.RBN RBN { get; set; }

        public Data.MRBN MRBN { get; set; }

        public Log Log { get; set; }

        /// <summary>
        ///     Конфиг серверов
        /// </summary>
        public Data.Server Server { get; set; }
    }
}
using BalancerLogger.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Helpers.Config
{
    /// <summary>
    /// Класс для упарвления конфигурационным файлом.
    /// </summary>
    public class ManageConfig
    {
        #region Read

        /// <summary>
        /// Метод загрузки значения LogLevel.
        /// </summary>
        /// <returns></returns>
        public LogLevel LoadLogLevelFromConfig()
        {
            ConfigHelper confHelper = new ConfigHelper();
            string level = confHelper.GetConfigFromKey("LogLevel");
            LogLevel lvl;

            switch (level)
            {
                case "DEBUG":
                    {
                        lvl = LogLevel.DEBUG;
                        break;
                    }
                case "INFO":
                    {
                        lvl = LogLevel.INFO;
                        break;
                    }
                case "WARN":
                    {
                        lvl = LogLevel.WARN;
                        break;
                    }
                case "ERROR":
                    {
                        lvl = LogLevel.ERROR;
                        break;
                    }
                case "FATAL":
                    {
                        lvl = LogLevel.FATAL;
                        break;
                    }
                default:
                    {
                        lvl = LogLevel.DEBUG;
                        break;
                    }
            }
            return lvl;
        }

        /// <summary>
        /// Метод загрузки значения LogMode.
        /// </summary>
        /// <returns></returns>
        public LogMode LoadLogModeFromConfig()
        {
            ConfigHelper confHelper = new ConfigHelper();
            string mode = confHelper.GetConfigFromKey("LogMode");
            LogMode m;

            switch (mode)
            {
                case "SINGLE":
                    {
                        m = LogMode.SINGLE;
                        break;
                    }
                case "MULTIPLE":
                    {
                        m = LogMode.MULTIPLE;
                        break;
                    }
                default:
                    {
                        m = LogMode.MULTIPLE;
                        break;
                    }
            }
            return m;
        }

        /// <summary>
        /// Метод загрузки директории логирования.
        /// </summary>
        /// <returns>Директория логирования.</returns>
        public string LoadLogDirFromConfig()
        {
            ConfigHelper confHelper = new ConfigHelper();
            return confHelper.GetConfigFromKey("LogDir");
        }
        #endregion Read

        #region Write
        #endregion Write
    }
}

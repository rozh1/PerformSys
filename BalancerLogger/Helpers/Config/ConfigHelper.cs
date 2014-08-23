using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Helpers.Config
{
    /// <summary>
    /// Класс хелпер, помогающий считать конфигураөионный файл. 
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// Метод для получения значения свойства по ключю.
        /// </summary>
        /// <param name="key">Название ключа.</param>
        /// <returns>Значение свойства.</returns>
        public string GetConfigFromKey(string key)
        {
            string exeConfigPath = this.GetType().Assembly.Location;
            Configuration config = null;
          
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch { }

            if (config != null)
            {
                return GetAppSetting(config, key);
            }
            return null;
        }

        /// <summary>
        /// Метод получения значения свойства из конфигурации.
        /// </summary>
        /// <param name="config">Конфигуриация.</param>
        /// <param name="key">Ключ.</param>
        /// <returns>Значение свойства.</returns>
        private string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }
    }
}

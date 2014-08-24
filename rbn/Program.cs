using System.IO;
using System.Text;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using rbn.Config;
using rbn.Properties;

namespace rbn
{
    internal static class Program
    {
        private static void Main()
        {
            const string configFilePath = "rbnConfig.xml";
            if (!File.Exists(configFilePath))
            {
                RBNConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            RBNConfig.Load(configFilePath);
            Logger.Write(Config.RBNConfig.Instance.Log.LogFile,
                new StringLogData("Сервер запущен"), 
                LogLevel.INFO);
            
            new Server((int)RBNConfig.Instance.RBN.Port);
        }
    }
}
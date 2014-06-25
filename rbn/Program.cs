using System.IO;
using System.Text;
using Balancer.Common;
using rbn.Config;
using rbn.Properties;

namespace rbn
{
    internal static class Program
    {
        private static void Main()
        {
            Logger.SetLogFile("rbnLog.txt");
            Logger.Write("Сервер запущен");

            const string configFilePath = "rbnConfig.xml";
            if (!File.Exists(configFilePath))
            {
                RBNConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            RBNConfig.Load(configFilePath);
            
            new Server((int)RBNConfig.Instance.RBN.Port);
        }
    }
}
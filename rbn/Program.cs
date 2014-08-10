using System.IO;
using System.Text;
using Balancer.Common;
using Balancer.Common.Logger;
using rbn.Config;
using rbn.Properties;

namespace rbn
{
    internal static class Program
    {
        private static void Main()
        {
            Logger.SetLogFile("rbnLog.txt");
            Logger.SetCsvLogFile("statsRBN.csv");
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
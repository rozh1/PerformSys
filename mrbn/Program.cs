using System.IO;
using System.Text;
using Balancer.Common;
using mrbn.Config;
using mrbn.Config.Data;
using mrbn.Properties;

namespace mrbn
{
    static class Program
    {
        static void Main(string[] args)
        {
            Logger.SetLogFile("mrbnLog.txt");
            Logger.Write("Сервер запущен");

            const string configFilePath = "mrbnConfig.xml";
            if (!File.Exists(configFilePath))
            {
                MRBNConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            MRBNConfig.Load(configFilePath);

            new Server((int)MRBNConfig.Instance.MRBN.Port);
        }
    }
}

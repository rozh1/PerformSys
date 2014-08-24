using System.IO;
using System.Text;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using mrbn.Config;
using mrbn.Properties;

namespace mrbn
{
    static class Program
    {
        static void Main(string[] args)
        {

            const string configFilePath = "mrbnConfig.xml";
            if (!File.Exists(configFilePath))
            {
                MRBNConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            MRBNConfig.Load(configFilePath);

            Logger.Write(MRBNConfig.Instance.LogFile,new StringLogData("Сервер запущен"), LogLevel.INFO);

            new Server((int)MRBNConfig.Instance.MRBN.Port);
        }
    }
}

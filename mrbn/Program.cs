using System.IO;
using System.Text;
using mrbn.Config;
using mrbn.Properties;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;

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

            Logger.Configure(MRBNConfig.Instance.Log);
            Logger.Write(MRBNConfig.Instance.Log.LogFile,new StringLogData("Сервер запущен"), LogLevel.INFO);

            new Server((int)MRBNConfig.Instance.MRBN.Port);
        }
    }
}

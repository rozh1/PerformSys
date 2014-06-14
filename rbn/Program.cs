using System;
using System.IO;
using Balancer.Common;
using rbn.Properties;
using rbn.ServersHandler;

namespace rbn
{
    internal class Program
    {
        private static void Main()
        {
            Logger.SetLogFile("rbnLog.txt");
            Logger.Write("Сервер запущен");

            string configFilePath = Environment.CurrentDirectory + "/rbn.cfg";
            ConfigFile.SetConfigPath(configFilePath);
            if (!File.Exists(configFilePath)) ConfigFile.SaveSettings(Resources.defaultConfig);
            ConfigFile.LoadSettings();

            Settings.Init();

            Servers.Init();

            new Server(int.Parse(ConfigFile.GetConfigValue("RBN_Port")));
        }
    }
}
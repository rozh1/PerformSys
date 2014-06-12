using System;
using System.IO;
using Balancer.Common;
using rbn.ServersHandler;

namespace rbn
{
    internal class Program
    {
        private static void Main()
        {
            Logger.Write("Сервер запущен");

            string configFilePath = Environment.CurrentDirectory + "/rbn.cfg";
            ConfigFile.SetConfigPath(configFilePath);
            if (!File.Exists(configFilePath)) ConfigFile.SaveSettings(Properties.Resources.defaultConfig);
            ConfigFile.LoadSettings();

            Settings.Init();

            Servers.Init();

            new Server(int.Parse(ConfigFile.GetConfigValue("RBN_Port")));
        }
    }
}
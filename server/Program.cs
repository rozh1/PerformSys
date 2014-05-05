using System;
using System.IO;
using System.Threading;
using Balancer.Common;

namespace server
{
    internal class Program
    {
        private static void Main()
        {
            Logger.Write("Сервер запущен");

            string configFilePath = "server.cfg";
            ConfigFile.SetConfigPath(configFilePath);
            if (!File.Exists(configFilePath)) ConfigFile.SaveSettings(Properties.Resources.defaultConfig);
            ConfigFile.LoadSettings();

            int maxThreadsCount = Environment.ProcessorCount*2;

            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            ThreadPool.SetMinThreads(1, 1);

            if (!DataBase.DB.Init())
            {
                Logger.Write("Ошибка подключения к БД. Выход.");
                return;
            }

            Logger.Write("Сервер конфигурирован");
            new Server(int.Parse(ConfigFile.GetConfigValue("ServerPort")));
        }
    }
}
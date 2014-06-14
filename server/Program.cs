using System;
using System.IO;
using System.Threading;
using Balancer.Common;
using server.DataBase;
using server.Properties;

namespace server
{
    internal class Program
    {
        private static void Main()
        {
            Logger.SetLogFile("serverLog.txt");
            Logger.Write("Сервер запущен");

            const string configFilePath = "server.cfg";
            ConfigFile.SetConfigPath(configFilePath);
            if (!File.Exists(configFilePath)) ConfigFile.SaveSettings(Resources.defaultConfig);
            ConfigFile.LoadSettings();

            int maxThreadsCount = Environment.ProcessorCount;

            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            ThreadPool.SetMinThreads(1, 1);

            if (!Database.Init())
            {
                Logger.Write("Ошибка подключения к БД. Выход.");
                return;
            }

            Logger.Write("Сервер конфигурирован");
            new Server(int.Parse(ConfigFile.GetConfigValue("ServerPort")));
        }
    }
}
using System;
using System.Threading;

namespace server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.Write("Сервер запущен");
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
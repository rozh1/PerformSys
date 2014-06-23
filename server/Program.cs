using System;
using System.IO;
using System.Text;
using System.Threading;
using Balancer.Common;
using server.Config;
using server.Config.Data;
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

            const string configFilePath = "serverConfig.xml";
            if (!File.Exists(configFilePath))
            {
                ServerConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            ServerConfig.Load(configFilePath);

            int maxThreadsCount = Environment.ProcessorCount;

            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            ThreadPool.SetMinThreads(1, 1);

            if (ServerConfig.Instance.Server.WorkMode == WorkMode.Normal)
            {
                if (!Database.Init())
                {
                    Logger.Write("Ошибка подключения к БД. Выход.");
                    return;
                }
            }

            Logger.Write("Сервер конфигурирован");
            new Server();
        }
    }
}
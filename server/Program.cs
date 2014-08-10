using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Balancer.Common.Logger;
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
            Logger.SetCsvLogFile("statsServer.csv");
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

            var databases = new Dictionary<int, MySqlDb>();

            if (ServerConfig.Instance.Server.WorkMode == WorkMode.Normal)
            {
                foreach (var dataBaseConfig in ServerConfig.Instance.DataBase)
                {
                    var database = new MySqlDb(
                        dataBaseConfig.UserName,
                        dataBaseConfig.Password,
                        dataBaseConfig.DataBaseName,
                        dataBaseConfig.Host,
                        dataBaseConfig.Port);

                    if (!database.MySqlConnectionOpen())
                    {
                        Logger.Write("Ошибка подключения к БД. Выход.");
                        return;
                    }
                    databases.Add(dataBaseConfig.RegionId, database);
                }
            }

            Logger.Write("Сервер конфигурирован");
            new Server(databases);
        }
    }
}
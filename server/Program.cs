using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
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
            const string configFilePath = "serverConfig.xml";
            if (!File.Exists(configFilePath))
            {
                ServerConfig.Load(
                    new MemoryStream(Encoding.UTF8.GetBytes(Resources.defaultConfig))).Save(configFilePath);
            }
            ServerConfig.Load(configFilePath);

            Logger.Write(ServerConfig.Instance.Log.LogFile, 
                new StringLogData("Сервер запущен"), 
                LogLevel.INFO);

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
                        Logger.Write(ServerConfig.Instance.Log.LogFile, 
                            new StringLogData("Ошибка подключения к БД. Выход."), 
                            LogLevel.FATAL);
                        return;
                    }
                    databases.Add(dataBaseConfig.RegionId, database);
                }
            }

            Logger.Write(ServerConfig.Instance.Log.LogFile, 
                new StringLogData("Сервер конфигурирован"), 
                LogLevel.INFO);
            new Server(databases);
        }
    }
}
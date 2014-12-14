using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Utils.CommandLineArgsParser;
using server.Config;
using server.Config.Data;
using server.DataBase;

namespace server
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string configFilePath = "serverConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            ServerConfig.Load(configFilePath);

            Logger.Configure(ServerConfig.Instance.Log);
            Logger.Write(ServerConfig.Instance.Log.LogFile,
                new StringLogData("Сервер запущен"),
                LogLevel.INFO);

            int maxThreadsCount = Environment.ProcessorCount;

            ThreadPool.SetMaxThreads(maxThreadsCount, maxThreadsCount);

            ThreadPool.SetMinThreads(1, 1);

            var databases = new Dictionary<int, MySqlDb>();

            if (ServerConfig.Instance.Server.WorkMode == WorkMode.Normal)
            {
                foreach (Config.Data.DataBase dataBaseConfig in ServerConfig.Instance.DataBase)
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

            var comandLineArguments = new List<ComandLineArgument>
            {
                new ComandLineArgument("--host", new[] {"-h"}),
                new ComandLineArgument("--port", new[] {"-p"})
            };

            var commandLineArgumentsParser = new CommandLineArgumentsParser(comandLineArguments.ToArray());
            ComandLineArgument[] parseResult = commandLineArgumentsParser.Parse(args);

            foreach (ComandLineArgument comandLineArgument in parseResult)
            {
                if (comandLineArgument.IsDefined)
                {
                    switch (comandLineArgument.Argument)
                    {
                        case "--host":
                            ServerConfig.Instance.Server.RBN.Host = comandLineArgument.Value;
                            break;
                        case "--port":
                            int port;
                            if (int.TryParse(comandLineArgument.Value, out port))
                            {
                                ServerConfig.Instance.Server.RBN.Port = (uint) port;
                            }
                            break;
                    }
                }
            }

            new Server(databases);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Utils.CommandLineArgsParser;
using router.Config;

namespace router
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string configFilePath = "routerConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            RouterConfig.Load(configFilePath);

            Logger.Configure(RouterConfig.Instance.Log);
            Logger.Write(RouterConfig.Instance.Log.LogFile,
                new StringLogData("Сервер запущен"),
                LogLevel.INFO);
            
            Logger.Write(RouterConfig.Instance.Log.LogFile,
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
                            RouterConfig.Instance.Router.RBN.Host = comandLineArgument.Value;
                            break;
                        case "--port":
                            int port;
                            if (int.TryParse(comandLineArgument.Value, out port))
                            {
                                RouterConfig.Instance.Router.RBN.Port = (uint) port;
                            }
                            break;
                    }
                }
            }

            new Server();
        }
    }
}
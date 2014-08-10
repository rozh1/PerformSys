using System;
using System.Diagnostics;
using Balancer.Common;
using client.ComandLineParamsParser;

namespace client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine(@"Использование эмулятора клиентов {0}:", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"{0} --clients 10 --queries 5 --host localhost --port 3409", AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine(@"    --clients, -c   - количество эмулируемых клиентов");
                Console.WriteLine(@"    --queries, -q   - количество запросов от одного клиента");
                Console.WriteLine(@"    --host, -h      - адрес балансировщика");
                Console.WriteLine(@"    --port, -p      - порт балансировщика");
                Environment.Exit(-1);
            }

            var parser = new Parser(args);

            if (!string.IsNullOrEmpty(parser.ErrorText))
            {
                Logger.Write(parser.ErrorText);
                Environment.Exit(-1);
            }

            Config.Config config = parser.GetConfig();
            Logger.SetCsvLogFile("statsClients.csv");

            Debug.Assert(config.ClientCount != null, "config.ClientCount != null");
            var clients = new Client[(int)config.ClientCount];
            
            for (int i = 0; i < (int)config.ClientCount; i++)
            {
                clients[i] = new Client(config, i, (i%14) + 1);
            }
        }
    }
}
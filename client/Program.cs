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
                Console.WriteLine(string.Format("Использование эмулятора клиентов {0}:", AppDomain.CurrentDomain.FriendlyName));
                Console.WriteLine(string.Format("{0} --clients 10 --queries 5 --host localhost --port 3409", AppDomain.CurrentDomain.FriendlyName));
                Console.WriteLine(string.Format("\t--clients, -c\t- количество эмулируемых клиентов"));
                Console.WriteLine(string.Format("\t--queries, -q\t- количество запросов от одного клиента"));
                Console.WriteLine(string.Format("\t--host, -h\t- адрес балансировщика"));
                Console.WriteLine(string.Format("\t--port, -p\t- порт балансировщика"));
                Environment.Exit(-1);
            }

            Parser parser = new Parser(args);

            if (!string.IsNullOrEmpty(parser.ErrorText))
            {
                Logger.Write(parser.ErrorText);
                Environment.Exit(-1);
            }

            Config.Config config = parser.GetConfig();

            Debug.Assert(config.ClientCount != null, "config.ClientCount != null");
            var clients = new Client[(int)config.ClientCount];
            
            for (int i = 0; i < (int)config.ClientCount; i++)
            {
                clients[i] = new Client(config, i, (i%14) + 1);
            }
        }
    }
}
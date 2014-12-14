using System.IO;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using client.Config;
using client.QuerySequence;

namespace client
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string configFilePath = "clientConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            ClientConfig.Load(configFilePath);

            Logger.Configure(ClientConfig.Instance.Log);
            var scenario = ClientConfig.Instance.Scenario;
            var querySequenceManager = new QuerySequenceManager(ClientConfig.Instance);
            var clients = new Client[scenario.ClientCount];

            Logger.Write(ClientConfig.Instance.Log.LogFile, new StringLogData("Запуск клиентов..."), LogLevel.INFO);

            int queriesCount = 14;
            for (int i = 0; i < scenario.ClientCount; i++)
            {
                var qSeq = querySequenceManager.GetQuerySequence((i % queriesCount) + 1, i + 1, queriesCount);
                clients[i] = new Client(ClientConfig.Instance, (i + 1), qSeq);
            }

            Logger.Write(ClientConfig.Instance.Log.LogFile, new StringLogData("Клиенты запущены"), LogLevel.INFO);
        }
    }
}
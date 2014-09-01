using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Data;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using client.Config.Data;
using client.Properties;

namespace client
{
    /// <summary>
    ///     Эмулятор клиента.
    /// </summary>
    internal class Client
    {
        private readonly string _address;
        private readonly int _clientId;
        private readonly Config.Config _config;
        private readonly PacketTransmitHelper _packetTransmitHelper;
        private readonly int _port;
        private readonly QuerySequence.QuerySequence _querySequence;
        private readonly ScenarioStep[] _scenarioSteps;
        private int _queryNumber;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="config">Конфигурация клиента.</param>
        /// <param name="clientId">Идентификатор клиента.</param>
        /// <param name="querySequence">Последовательность запросов.</param>
        /// <param name="scenarioSteps">Шаги сценария</param>
        public Client(Config.Config config,
            int clientId,
            QuerySequence.QuerySequence querySequence,
            ScenarioStep[] scenarioSteps)
        {
            _address = config.BalancerHost;
            Debug.Assert(config.BalancerPort != null, "config.BalancerPort != null");
            _port = (int) config.BalancerPort;
            _clientId = clientId;
            _scenarioSteps = scenarioSteps;
            _querySequence = querySequence;
            _config = config;
            _packetTransmitHelper = new PacketTransmitHelper(_config.Log.LogFile);

            var t = new Thread(ClientThread);
            t.Start();
        }

        /// <summary>
        ///     поток эмулятора клиента
        /// </summary>
        private void ClientThread()
        {
            int loopNumber = 0;
            var tcpClient = new TcpClient();
            var clientStatsData = new ClientStatsData();

            tcpClient.Connect(_address, _port);

            var scenarioStepsManager = new ScenarioStepsManager(_scenarioSteps);

            while (true)
            {
                ScenarioActions scenarioAction = scenarioStepsManager.GetCurrentScenarioAction();

                if (scenarioAction == ScenarioActions.Sleep)
                {
                    Thread.Sleep(100);
                    continue;
                }
                if (scenarioAction == ScenarioActions.Stop)
                {
                    break;
                }

                loopNumber++;

                if (tcpClient.Connected)
                {
                    _queryNumber = _querySequence.GetNextQueryNumber();
                    string query = Resources.ResourceManager.GetString("q" + _queryNumber);

                    var dbRequestPacket = new DbRequestPacket(query, _queryNumber)
                    {
                        ClientId = (uint) _clientId
                    };

                    _packetTransmitHelper.Send(dbRequestPacket.GetPacket(), tcpClient.GetStream());

                    DateTime startTime = DateTime.UtcNow;

                    Packet packet = _packetTransmitHelper.Recive(tcpClient.GetStream());

                    //var dt = (DataTable)SerializeMapper.Deserialize(packet.Data);
                    //
                    //string answer = "";
                    //for (int i = 0; i < dt.Columns.Count; i++)  answer += dt.Columns[i].ColumnName + "\t";
                    //for (int j = 0; j < dt.Rows.Count; j++)
                    //{
                    //    answer += "\n";
                    //    for (int i = 0; i < dt.Columns.Count; i++) answer += dt.Rows[j][i] + "\t";
                    //}

                    TimeSpan queryTime = DateTime.UtcNow - startTime;
                    clientStatsData.WaitTime += queryTime;
                    clientStatsData.Answer = null; //answer;

                    Logger.Write(_config.Log.LogFile,
                        new StringLogData(string.Format(@"Клиент: {0}	Запрос: {1}	Время выполнения: {2}", _clientId,
                            loopNumber, queryTime)),
                        LogLevel.INFO);

                    _config.LogStats.ClientNumber = _clientId;
                    _config.LogStats.ClientQueryNumber = loopNumber;
                    _config.LogStats.QueryNumber = _queryNumber;
                    _config.LogStats.QueryTime = queryTime;

                    Logger.Write(_config.Log.StatsFile, _config.LogStats, LogLevel.INFO);
                }
            }
            tcpClient.Close();

            Logger.Write(_config.Log.LogFile,
                new StringLogData(string.Format(@"Клиент: {0}	Общее время работы: {1}", _clientId,
                    clientStatsData.WaitTime)),
                LogLevel.INFO);
        }
    }
}
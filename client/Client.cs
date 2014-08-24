using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Logger.Enums;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using client.Properties;

namespace client
{
    /// <summary>
    ///     эмулятор клиента
    /// </summary>
    internal class Client
    {
        private readonly string _address;
        private readonly Config.Config _config;
        private readonly int _number;
        private readonly int _port;
        private readonly int _queryNumber;
        private readonly PacketTransmitHelper _packetTransmitHelper;
        private ClientStatsData _clientStatsData;

        public Client(Config.Config config, int number, int queryNumber)
        {
            _address = config.BalancerHost;
            Debug.Assert(config.BalancerPort != null, "config.BalancerPort != null");
            _port = (int) config.BalancerPort;
            _number = number;
            _queryNumber = queryNumber;
            _config = config;
            _packetTransmitHelper = new PacketTransmitHelper("clentLog.txt");
            var t = new Thread(ClientThread);
            t.Start();
        }

        /// <summary>
        ///     поток эмулятора клиента
        /// </summary>
        private void ClientThread()
        {
            _clientStatsData = new ClientStatsData();

            var tcpClient = new TcpClient();
            tcpClient.Connect(_address, _port);
            for (int i = 0; i < _config.QueryCount; i++)
            {
                if (tcpClient.Connected)
                {
                    string query = Resources.ResourceManager.GetString("q" + _queryNumber);

                    var dbRequestPacket = new DbRequestPacket(query, _queryNumber);
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
                    _clientStatsData.WaitTime += queryTime;
                    _clientStatsData.Answer = null; //answer;
                    Console.WriteLine(@"Клиент: {0}	Запрос: {1}	Время выполнения: {2}", _number, i, queryTime);

                    _config.LogStats.ClientNumber = _number;
                    _config.LogStats.ClientQueryNumber = i;
                    _config.LogStats.QueryNumber = _queryNumber;
                    _config.LogStats.QueryTime = queryTime;

                    Logger.Write("statsClient.csv", _config.LogStats, LogLevel.INFO);
                }
            }
            tcpClient.Close();
            Console.WriteLine(@"Клиент: {0}	Общее время работы: {1}", _number, _clientStatsData.WaitTime);
        }
    }
}
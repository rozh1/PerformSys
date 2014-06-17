using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using client.Properties;

namespace client
{
    /// <summary>
    ///     эмулятор клиента
    /// </summary>
    internal class Client
    {
        private readonly string _address;
        private readonly int _number;
        private readonly int _port;
        private readonly int _queryNumber;
        public ClientStatsData ClientStatsData;
        private Config.Config _config;

        public Client(Config.Config config, int number, int queryNumber)
        {
            _address = config.BalancerHost;
            Debug.Assert(config.BalancerPort != null, "config.BalancerPort != null");
            _port = (int)config.BalancerPort;
            _number = number;
            _queryNumber = queryNumber;
            _config = config;
            var t = new Thread(ClientThread);
            t.Start();
        }

        /// <summary>
        ///     поток эмулятора клиента
        /// </summary>
        private void ClientThread()
        {
            ClientStatsData = new ClientStatsData();

            var tcpClient = new TcpClient();
            tcpClient.Connect(_address, _port);
            for (int i = 0; i < _config.QueryCount; i++)
            {
                if (tcpClient.Connected)
                {
                    string query = Resources.ResourceManager.GetString("q" + _queryNumber);
                    var dbRequestPacket = new DbRequestPacket(query, _queryNumber);
                    Byte[] requestPacket = dbRequestPacket.GetPacket().ToBytes();
                    tcpClient.GetStream().Write(requestPacket, 0, requestPacket.Length);

                    DateTime startTime = DateTime.Now;

                    var buffer = new byte[1400];
                    Packet packet;
                    do
                    {
                        string packetData = "";

                        int count;
                        while ((count = tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                        {
                            packetData += Encoding.ASCII.GetString(buffer, 0, count);
                            if (packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal) >= 0) break;
                        }
                        packet = new Packet(packetData);
                    } while (packet.Type != PacketType.Answer);

                    //var dt = (DataTable)SerializeMapper.Deserialize(packet.Data);
                    //
                    //string answer = "";
                    //for (int i = 0; i < dt.Columns.Count; i++)  answer += dt.Columns[i].ColumnName + "\t";
                    //for (int j = 0; j < dt.Rows.Count; j++)
                    //{
                    //    answer += "\n";
                    //    for (int i = 0; i < dt.Columns.Count; i++) answer += dt.Rows[j][i] + "\t";
                    //}

                    ClientStatsData.WaitTime = DateTime.Now - startTime;
                    ClientStatsData.Answer = null; //answer;
                    Console.WriteLine("Клиент: " +_number + "\tЗапрос: " + i + "\tВремя выполнения: " + ClientStatsData.WaitTime);
                }
            }
            tcpClient.Close();
            Console.WriteLine("Клиент: " +_number + "\tОбщее время работы: " + ClientStatsData.WaitTime);
        }
    }
}
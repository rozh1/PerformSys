using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;

namespace client
{
    class Client
    {
        private string _address;
        private int _port;
        public ClientStatsData clientStatsData;

        public Client(string address, int port)
        {
            _address = address;
            _port = port;
            Thread t = new Thread(new ThreadStart(ClientThread));
            t.Start();
        }

        void ClientThread()
        {
            clientStatsData = new ClientStatsData();
            var sw = new Stopwatch();

            var tcpClient = new TcpClient();
            tcpClient.Connect(_address, _port);
            if (tcpClient.Connected)
            {
                string query = "select * from IP;";
                var dbRequestPacket = new DbRequestPacket(query);
                Byte[] requestPacket = dbRequestPacket.GetPacket().ToBytes();
                tcpClient.GetStream().Write(requestPacket, 0, requestPacket.Length);

                sw.Start();

                var buffer = new byte[1400];
                Packet packet;
                do
                {
                    string packetData = "";

                    int count;
                    while ((count = tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    {
                        packetData += Encoding.ASCII.GetString(buffer, 0, count);
                        if (packetData.IndexOf("\n\r", StringComparison.Ordinal) >= 0) break;
                    }
                    packet = new Packet(packetData);
                } while (packet.Type != PacketType.Answer);

                sw.Stop();

                var dt = (DataTable)SerializeMapper.Deserialize(packet.Data);

                string answer = "";
                for (int i = 0; i < dt.Columns.Count; i++)  answer += dt.Columns[i].ColumnName + "\t";
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    answer += "\n";
                    for (int i = 0; i < dt.Columns.Count; i++) answer += dt.Rows[j][i] + "\t";
                }

                clientStatsData.WaitTime = sw.ElapsedMilliseconds;
                clientStatsData.Answer = answer;
            }
            tcpClient.Close();
            Console.WriteLine(clientStatsData.WaitTime);
        }
    }
}

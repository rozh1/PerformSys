using System;
using System.Data;
using System.Diagnostics;
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
        private int _number;
        public ClientStatsData ClientStatsData;

        public Client(string address, int port, int number)
        {
            _address = address;
            _port = port;
            _number = number;
            Thread t = new Thread(ClientThread);
            t.Start();
        }

        void ClientThread()
        {
            ClientStatsData = new ClientStatsData();
            var sw = new Stopwatch();

            var tcpClient = new TcpClient();
            tcpClient.Connect(_address, _port);
            if (tcpClient.Connected)
            {
                const string query = 
@"SELECT
        S_ACCTBAL,
        S_NAME,
        N_NAME,
        P_PARTKEY,
        P_MFGR,
        S_ADDRESS,
        S_PHONE,
        S_COMMENT
FROM
        PART,
        SUPPLIER,
        PARTSUPP,
        NATION,
        REGION
WHERE
        P_PARTKEY = PS_PARTKEY
        AND S_SUPPKEY = PS_SUPPKEY
        AND P_SIZE = 29
        AND P_TYPE LIKE '%BRASS'
        AND S_NATIONKEY = N_NATIONKEY
        AND N_REGIONKEY = R_REGIONKEY
        AND R_NAME = 'MIDDLE EAST'
        AND PS_SUPPLYCOST = (
                SELECT
                        MIN(PS_SUPPLYCOST)
                FROM
                        PARTSUPP,
                        SUPPLIER,
                        NATION,
                        REGION
                WHERE
                        P_PARTKEY = PS_PARTKEY
                        AND S_SUPPKEY = PS_SUPPKEY
                        AND S_NATIONKEY = N_NATIONKEY
                        AND N_REGIONKEY = R_REGIONKEY
                        AND R_NAME = 'MIDDLE EAST'
        )
ORDER BY
        S_ACCTBAL DESC,
        N_NAME,
        S_NAME,
        P_PARTKEY;";
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
                        if (packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal) >= 0) break;
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

                ClientStatsData.WaitTime = sw.ElapsedMilliseconds;
                ClientStatsData.Answer = answer;
            }
            tcpClient.Close();
            Console.WriteLine(_number + "\t" + ClientStatsData.WaitTime);
        }
    }
}

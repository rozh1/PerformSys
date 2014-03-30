using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Balancer.Packet;
using Balancer.Packet.Packets;


namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new Stopwatch();

            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(args[0], int.Parse(args[1]));
            if (tcpClient.Connected)
            {
                byte[] buf = new byte[100];
                tcpClient.GetStream().Read(buf, 0, buf.Length);
                string query = Console.ReadLine();
                DbRequestPacket dbRequestPacket = new DbRequestPacket(query);
                Byte[] requestPacket = dbRequestPacket.GetPacket().ToBytes();
                tcpClient.GetStream().Write(requestPacket, 0, requestPacket.Length);

                sw.Start();

                string packetData = "";
                var buffer = new byte[1400];
                int count;
                Packet packet;
                do
                {
                    packetData = "";

                    while ((count = tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                    {
                        packetData += Encoding.ASCII.GetString(buffer, 0, count);
                        if (packetData.IndexOf("\n\r", System.StringComparison.Ordinal) >= 0) break;
                    }
                    packet = new Packet(packetData);
                } while (packet.Type!=PacketType.Answer);

                sw.Stop();

                DataTable dt = (DataTable)SerializeMapper.Deserialize(packet.Data);

                for (int i = 0; i < dt.Columns.Count; i++) Console.Write(dt.Columns[i].ColumnName + "\t");
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    Console.WriteLine();
                    for (int i = 0; i < dt.Columns.Count; i++) Console.Write(dt.Rows[j][i].ToString() + "\t");
                }
                Console.WriteLine();
                Console.WriteLine("Время выполнения: " + sw.ElapsedMilliseconds + " мс");
            } 
            tcpClient.Close();
            Console.ReadKey();
        }
    }
}

using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Packet;
using Balancer.Packet.Packets;
using server.DataBase;

namespace server
{
    internal class Server
    {
        private readonly TcpListener _listener;

        private readonly TcpClient _tcpClient;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);
            while (true)
            {
                _tcpClient = _listener.AcceptTcpClient();

                Logger.Write("Принято соединие");

                while (_tcpClient.Connected)
                {
                    int workerThreads, completionPortThreads;
                    ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
                    var sp = new StatusPacket((workerThreads <= Environment.ProcessorCount*2));
                    Byte[] statusPacket = sp.GetPacket().ToBytes();
                    try
                    {
                        if (_tcpClient.Connected)
                            _tcpClient.GetStream().Write(statusPacket, 0, statusPacket.Length);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Исключение при отсылке статуса: " + ex.Message);
                    }
                    Logger.Write("Отослан статус");

                    string packetData = "";
                    var buffer = new byte[1400];
                    int count;

                    try
                    {
                        while ((count = _tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)

                        {
                            packetData += Encoding.ASCII.GetString(buffer, 0, count);
                            if (packetData.IndexOf("\n\r", System.StringComparison.Ordinal) >= 0) break;
                        }
                        ThreadPool.QueueUserWorkItem(MySqlWorker, packetData);
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Исключение при чтении запроса");
                    }
                }
            }
        }

        private void MySqlWorker(object data)
        {
            var packetData = (string) data;
            var packet = new Packet(packetData);

            Logger.Write("Принят запрос: " + packet.Data);

            DataTable dt = null;
            if (packet.Type == PacketType.Request)
            {
                Logger.Write("Запрос начал выполнение");
                var sw = new Stopwatch();
                sw.Start();
                dt = DB.CustomQuery(packet.Data);
                sw.Stop();
                Logger.Write("Запрос выполнен за " + sw.ElapsedMilliseconds + " мс");
            }
            if (dt != null)
            {
                Logger.Write("Отправка результата клиенту");
                DbAnswerPacket dbAnswerPacket = new DbAnswerPacket(dt);
                Byte[] answerPacket = dbAnswerPacket.GetPacket().ToBytes();
                _tcpClient.GetStream().Write(answerPacket, 0, answerPacket.Length);
            }
        }

        /// <summary>
        /// убийца - деструктор
        /// </summary>
        ~Server()
        {
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
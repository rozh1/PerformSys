using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using server.Config.Data;
using server.DataBase;

namespace server
{
    /// <summary>
    ///     СЕРВЕР БД
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Слушатель новых соедиений
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        ///     текущее содинение
        /// </summary>
        private readonly TcpClient _tcpClient;

        /// <summary>
        ///     длина очереди на сервере
        /// </summary>
        private int _queueLength;

        /// <summary>
        ///     признак жизни потока слушателя
        /// </summary>
        private bool _serverIsLife;

        public Server(uint port)
        {
            _listener = new TcpListener(IPAddress.Any, (int)port);

            _listener.Start();

            _serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                _tcpClient = _listener.AcceptTcpClient();

                Logger.Write("Принято соединие");
                string packetData = "";

                while (_tcpClient.Connected)
                {
                    SendStatus();

                    var buffer = new byte[1400];

                    try
                    {
                        int count;
                        while ((count = _tcpClient.GetStream().Read(buffer, 0, buffer.Length)) > 0)
                        {
                            packetData += Encoding.ASCII.GetString(buffer, 0, count);
                            if (packetData.Contains(Packet.PacketEnd)) break;
                        }
                        while (packetData.Contains(Packet.PacketEnd))
                        {
                            int index = packetData.IndexOf(Packet.PacketEnd, StringComparison.Ordinal);
                            string onePacketData = packetData.Remove(index);
                            packetData =
                                packetData.Substring(
                                    index +
                                    Packet.PacketEnd.Length);

                            _queueLength++;
                            SendStatus();
                            ThreadPool.QueueUserWorkItem(MySqlWorker, onePacketData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write("Исключение при чтении запроса: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        ///     Отправка статуса сервера
        /// </summary>
        private void SendStatus()
        {
            bool status = (_queueLength <= Environment.ProcessorCount*2);
            var sp = new StatusPacket(status);
            Byte[] statusPacket = sp.GetPacket().ToBytes();
            try
            {
                _tcpClient.GetStream().Write(statusPacket, 0, statusPacket.Length);
            }
            catch (Exception ex)
            {
                Logger.Write("Исключение при отсылке статуса: " + ex.Message);
            }
            Logger.Write("Отослан статус " + status);
        }

        /// <summary>
        ///     Обработчик запроса в MySQL
        /// </summary>
        /// <param name="data"></param>
        private void MySqlWorker(object data)
        {
            var packetData = (string) data;
            var packet = new Packet(packetData);

            var requestPacket = new DbRequestPacket(packet.Data);

            Logger.Write("Принят запрос от клиента: " + requestPacket.ClientId);
            DataTable dt = null;
            if (packet.Type == PacketType.Request)
            {
                dt = ProcessQuery(requestPacket);
            }
            if (dt != null)
            {
                Logger.Write("Отправка результата клиенту " + requestPacket.ClientId);
                var dbAnswerPacket = new DbAnswerPacket(dt, requestPacket.QueryNumber, new PacketBase() { ClientId = requestPacket.ClientId, RegionId = requestPacket.RegionId});
                Packet answerPacket = dbAnswerPacket.GetPacket();
                Byte[] answerPacketBytes = answerPacket.ToBytes();
                _tcpClient.GetStream().Write(answerPacketBytes, 0, answerPacketBytes.Length);
            }
            _queueLength--;
            SendStatus();
        }

        private DataTable ProcessQuery(DbRequestPacket requestPacket)
        {
            DataTable dt = null;
            Logger.Write("Запрос начал выполнение");
            var sw = new Stopwatch();
            sw.Start();

            switch (Config.ServerConfig.Instance.Server.WorkMode)
            {
                case WorkMode.Normal:
                    dt = ProcessQueryWithMySQL(requestPacket);
                    break;
                case WorkMode.Simulation:
                    dt = ProcessQuerySimulated(requestPacket);
                    break;
            }

            sw.Stop();
            Logger.Write("Запрос " + requestPacket.QueryNumber + " выполнен за " + sw.ElapsedMilliseconds + " мс");
            return dt;
        }

        private DataTable ProcessQueryWithMySQL(DbRequestPacket requestPacket)
        {
            DataTable dt=null;
            try
            {
                dt = Database.CustomQuery(requestPacket.Query);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            return dt;
        }

        private DataTable ProcessQuerySimulated(DbRequestPacket requestPacket)
        {
            Thread.Sleep(Config.ServerConfig.Instance.SimulatedTimes[requestPacket.QueryNumber]);
            DataTable dt = new DataTable();
            dt.TableName = "randomTable";
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            return dt; 
        }

        /// <summary>
        ///     убийца - деструктор
        /// </summary>
        ~Server()
        {
            _serverIsLife = false;
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
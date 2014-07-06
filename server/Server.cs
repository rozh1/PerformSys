using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using server.Config;
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
        private bool _serverIsLife = true;

        public Server()
        {
            _tcpClient = new TcpClient();
            while (_serverIsLife)
            {
                try
                {
                    _tcpClient.Connect(
                        ServerConfig.Instance.Server.RBN.Host, 
                        (int)ServerConfig.Instance.Server.RBN.Port);
                }
                catch (Exception)
                {
                    Logger.Write("Не удалось подключиться. Переподключение");
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Write("Подключение установлено");

                SendDataBaseInfo();

                while (_tcpClient.Connected)
                {
                    SendStatus();
                    Packet onePacketData = PacketTransmitHelper.Recive(_tcpClient.GetStream());

                    if (onePacketData != null)
                    {
                        _queueLength++;
                        SendStatus();
                        ThreadPool.QueueUserWorkItem(MySqlWorker, onePacketData);
                    }
                    else
                    {
                        Logger.Write("Получен пустой пакет или разорвано соединение");
                    }
                }
                _tcpClient.Close();
                _tcpClient = new TcpClient();
            }
        }

        void SendDataBaseInfo()
        {
            DataTable dt = null;
            var requestPacket = new DbRequestPacket("SELECT table_name AS table_name, data_length FROM information_schema.tables WHERE table_schema=DATABASE();", 0);
            switch (ServerConfig.Instance.Server.WorkMode)
            {
                case WorkMode.Normal:
                    dt = ProcessQueryWithMySQL(requestPacket);
                    break;
                case WorkMode.Simulation:
                    dt = GenerateSimulatedSizesDataTable();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Нет такого режима работы");
            }

            var tableSizes = new Dictionary<string, UInt64>();
            foreach (DataRow row in dt.Rows)
            {
                if (!tableSizes.ContainsKey(row["table_name"].ToString()))
                    tableSizes.Add(row["table_name"].ToString(), Convert.ToUInt64(row["data_length"]));
            }

            if (dt != null)
            {
                Logger.Write("Отправка информации о БД РБНу ");
                var dataBaseInfoPacket = new DataBaseInfoPacket(tableSizes);
                PacketTransmitHelper.Send(dataBaseInfoPacket.GetPacket(), _tcpClient.GetStream());
            }
        }

        private DataTable GenerateSimulatedSizesDataTable()
        {
            var dt = new DataTable() {TableName = "sizes"};
            dt.Columns.Add("table_name");
            dt.Columns.Add("data_length");

            foreach (KeyValuePair<string,UInt64> pair in ServerConfig.Instance.SimulationSizes)
            {
                DataRow dr = dt.NewRow();
                dr[0] = pair.Key;
                dr[1] = pair.Value;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        ///     Отправка статуса сервера
        /// </summary>
        private void SendStatus()
        {
            bool status = (_queueLength < Environment.ProcessorCount*2);
            var sp = new ServerStatusPacket(status);
            if (_tcpClient.Connected)
            {
                PacketTransmitHelper.Send(sp.GetPacket(), _tcpClient.GetStream());
                Logger.Write("Отослан статус " + status);
            }
        }

        /// <summary>
        ///     Обработчик запроса в MySQL
        /// </summary>
        /// <param name="packetObj"></param>
        private void MySqlWorker(object packetObj)
        {
            DataTable dt = null;
            var packet = (Packet) packetObj;
            var requestPacket = new DbRequestPacket(packet.Data);

            Logger.Write("Принят запрос от клиента: " + requestPacket.ClientId);
            if (packet.Type == PacketType.Request)
            {
                dt = ProcessQuery(requestPacket);
            }
            if (dt != null)
            {
                Logger.Write("Отправка результата клиенту " + requestPacket.ClientId);
                var dbAnswerPacket = new DbAnswerPacket(dt, requestPacket.QueryNumber,
                    new PacketBase {ClientId = requestPacket.ClientId, RegionId = requestPacket.RegionId});
                PacketTransmitHelper.Send(dbAnswerPacket.GetPacket(), _tcpClient.GetStream());
            }
            _queueLength--;
            SendStatus();
        }

        /// <summary>
        ///     Обработчик запросов
        /// </summary>
        /// <param name="requestPacket">пакет запроса</param>
        /// <returns></returns>
        private DataTable ProcessQuery(DbRequestPacket requestPacket)
        {
            DataTable dt;
            Logger.Write("Запрос начал выполнение");
            var sw = new Stopwatch();
            sw.Start();

            switch (ServerConfig.Instance.Server.WorkMode)
            {
                case WorkMode.Normal:
                    dt = ProcessQueryWithMySQL(requestPacket);
                    break;
                case WorkMode.Simulation:
                    dt = ProcessQuerySimulated(requestPacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ServerConfig.Instance.Server.WorkMode Нет такого режима работы");
            }

            sw.Stop();
            Logger.Write("Запрос " + requestPacket.QueryNumber + " выполнен за " + sw.ElapsedMilliseconds + " мс");
            return dt;
        }

        /// <summary>
        ///     Обрабочик запроса с помощью MySQL
        /// </summary>
        /// <param name="requestPacket">пакет запроса</param>
        /// <returns></returns>
        private DataTable ProcessQueryWithMySQL(DbRequestPacket requestPacket)
        {
            DataTable dt = null;
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

        /// <summary>
        ///     Симулятор обрботки запросов
        /// </summary>
        /// <param name="requestPacket">пакет запроса</param>
        /// <returns></returns>
        private DataTable ProcessQuerySimulated(DbRequestPacket requestPacket)
        {
            var rand = new Random();
            Thread.Sleep(ServerConfig.Instance.SimulationParams[requestPacket.QueryNumber][0]);
            var data = new byte[ServerConfig.Instance.SimulationParams[requestPacket.QueryNumber][1]];

            rand.NextBytes(data);

            var dt = new DataTable {TableName = "randomTable"};
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = Encoding.ASCII.GetString(data);
            dt.Rows.Add(dr);
            return dt;
        }

        /// <summary>
        ///     убийца - деструктор
        /// </summary>
        ~Server()
        {
            _serverIsLife = false;
            if (_tcpClient != null)
            {
                if ( _tcpClient.Connected) _tcpClient.Close();
            }
        }
    }
}
﻿using System;
using System.Data;
using System.Diagnostics;
using System.Net;
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
            _listener = new TcpListener(IPAddress.Any, (int) port);
            _listener.Start();

            _serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                _tcpClient = _listener.AcceptTcpClient();

                Logger.Write("Принято соединие");

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
            }
        }

        /// <summary>
        ///     Отправка статуса сервера
        /// </summary>
        private void SendStatus()
        {
            bool status = (_queueLength < Environment.ProcessorCount*2);
            var sp = new StatusPacket(status);
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
            DataTable dt = null;
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
            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }
}
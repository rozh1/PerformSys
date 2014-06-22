﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using rbn.QueueHandler;
using rbn.ServersHandler;

namespace rbn
{
    /// <summary>
    ///     РБН
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Слушатель новых соединений (клиентов)
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        ///     признак жизн потока
        /// </summary>
        private bool _serverIsLife;

        /// <summary>
        /// Очередь регионального балансировщика
        /// </summary>
        private readonly RbnQueue _rbnQueue;

        /// <summary>
        /// Сервера региона
        /// </summary>
        private readonly Servers _servers;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            _rbnQueue = new RbnQueue();
            _servers = new Servers();
            _servers.AnswerRecivedEvent += _rbnQueue.ServerAnswer;
            _servers.SendRequestFromQueueEvent += _rbnQueue.SendRequestToServer;

            _serverIsLife = true;


            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                var t = new Thread(ClientThread);
                t.Start(tcpClient);
            }
        }

        /// <summary>
        ///     Поток обслуживания клиента
        /// </summary>
        /// <param name="param"></param>
        private void ClientThread(object param)
        {
            var tcpClient = (TcpClient) param;
            var client = new Client();
            while (tcpClient.Connected)
            {
                var packet = Balancer.Common.Utils.PacketTransmitHelper.Recive(tcpClient.GetStream());
                if (packet != null)
                {
                    var dbRequestPacket = new DbRequestPacket(packet.Data);
                    dbRequestPacket.GlobalId = Config.RBNConfig.Instance.RBN.GlobalId;
                    dbRequestPacket.RegionId = Config.RBNConfig.Instance.RBN.RegionId;

                    client.Connection = tcpClient;
                    client.RequestPacketData = dbRequestPacket.GetPacket().Data;

                    _rbnQueue.AddClient(client);
                }
                else
                {
                    tcpClient.Close();
                }
            }
            Logger.Write("Клиент отключен:");
            _rbnQueue.RemoveClient(client);
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
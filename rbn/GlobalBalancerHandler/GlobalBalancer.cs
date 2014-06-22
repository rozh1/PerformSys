using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Config;
using rbn.Interfaces;
using rbn.QueueHandler;

namespace rbn.GlobalBalancerHandler
{
    internal class GlobalBalancer : IServer
    {
        private Data.GlobalBalancer _globalBalancer;
        private Thread _globalBalancersThread;
        private TcpListener _listener;

        public event Action<IServer> SendRequestFromQueueEvent;

        /// <summary>
        ///     Отправка запроса серверу
        /// </summary>
        public bool SendRequest(QueueEntity queueEntity)
        {
            Data.GlobalBalancer globalBalancer = _globalBalancer;
            if (globalBalancer != null)
            {
                if (!globalBalancer.Connection.Connected) return false;
                var dbRequestPacket = new DbRequestPacket(queueEntity.RequestData);
                if (!PacketTransmitHelper.Send(dbRequestPacket.GetPacket(), globalBalancer.Connection.GetStream()))
                    return false;
                Logger.Write("Отправлен запрос от клиента " + queueEntity.ClientId);
            }
            else return false;
            return true;
        }

        /// <summary>
        ///     Событие получения ответа
        /// </summary>
        public event Action<int, DbAnswerPacket> AnswerRecivedEvent;

        private void Init()
        {
            var port = (int) RBNConfig.Instance.RBN.GlobalBalancerPort;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            TcpClient tcpClient = _listener.AcceptTcpClient();
            Logger.Write("Принято соединие");

            _globalBalancer = new Data.GlobalBalancer
            {
                Connection = tcpClient,
                Status = false,
                StatusRecived = false
            };
            _globalBalancersThread = new Thread(GlobalBalancerListenThread);
            _globalBalancersThread.Start(_globalBalancer);
        }

        private void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer) param;
            TcpClient connection = globalBalancer.Connection;

            while (connection.Connected)
            {
                Packet packet = PacketTransmitHelper.Recive(connection.GetStream());

                switch (packet.Type)
                {
                    case PacketType.TransmitRequest:
                        if (SendRequestFromQueueEvent != null) SendRequestFromQueueEvent(this);
                        break;
                    case PacketType.Answer:
                        var answer = new DbAnswerPacket(packet.Data);
                        if (AnswerRecivedEvent != null)
                            AnswerRecivedEvent((int) answer.ClientId, new DbAnswerPacket(packet.Data));
                        break;
                }
            }
            Init();
        }
    }
}
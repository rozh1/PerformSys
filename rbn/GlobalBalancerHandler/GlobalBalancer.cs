using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using rbn.QueueHandler;

namespace rbn.GlobalBalancerHandler
{
    internal class GlobalBalancers
    {
        private static GlobalBalancer _globalBalancer;
        private static Thread _globalBalancersThread;
        private static TcpListener _listener;
        private static bool _serverIsLife;

        public static void Init()
        {
            int port = (int)Config.RBNConfig.Instance.RBN.GlobalBalancerPort;

            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            _serverIsLife = true;

            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            while (_serverIsLife)
            {
                TcpClient tcpClient = _listener.AcceptTcpClient();
                Logger.Write("Принято соединие");

                _globalBalancer = new GlobalBalancer
                {
                    Connection = tcpClient,
                    Status = false,
                    StatusRecived = false
                };
                _globalBalancersThread = new Thread(GlobalBalancerListenThread);
                _globalBalancersThread.Start(_globalBalancer);
            }
        }

        private static void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (GlobalBalancer) param;
            TcpClient connection = globalBalancer.Connection;

            while (connection.Connected)
            {
                var packet = Balancer.Common.Utils.PacketTransmitHelper.Recive(connection.GetStream());

                switch (packet.Type)
                {
                    case PacketType.Status:
                        var sp = new StatusPacket(packet.Data);
                        globalBalancer.Status = sp.Status;
                        globalBalancer.StatusRecived = true;
                        if (globalBalancer.Status) RbnQueue.SendRequestToServer();
                        break;
                    case PacketType.Answer:
                        var answer = new DbAnswerPacket(packet.Data);
                        RbnQueue.ServerAnswer((int) answer.ClientId, packet.Data);
                        break;
                }
            }
        }
    }
}
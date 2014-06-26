using System;
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
    internal class GlobalBalancer : IServer, IDisposable
    {
        private readonly Data.GlobalBalancer _globalBalancer;

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

        public GlobalBalancer()
        {
            _globalBalancer = new Data.GlobalBalancer
            {
                Connection = null,
                Status = true,
                StatusRecived = false
            };
            var globalBalancersThread = new Thread(GlobalBalancerListenThread);
            globalBalancersThread.Start(_globalBalancer);
        }

        private void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer) param;
            TcpClient connection = new TcpClient();

            while (globalBalancer.Status)
            {
                try
                {
                    connection.Connect(
                        RBNConfig.Instance.MRBN.Host,
                        (int)RBNConfig.Instance.MRBN.Port);
                }
                catch (Exception)
                {
                    Logger.Write("Не удалось подключиться к межрегиональному балансировщику. Переподключение");
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Write("Подключение установлено");

                globalBalancer.Connection = connection;

                Thread statusSendThread = new Thread(StatusSendThread);
                statusSendThread.Start(globalBalancer);

                while (connection.Connected)
                {
                    Packet packet = PacketTransmitHelper.Recive(connection.GetStream());
                    if (packet != null)
                    {
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
                }
                connection.Close();
                connection = new TcpClient();
            }
        }

        private void StatusSendThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer)param;
            while (globalBalancer.Connection.Connected)
            {
                var packet = new RBNStatusPacket(1);
                packet.RegionId = Config.RBNConfig.Instance.RBN.RegionId;
                PacketTransmitHelper.Send(packet.GetPacket(), globalBalancer.Connection.GetStream());
                Thread.Sleep(100);
            }

        }

        public void Dispose()
        {
            _globalBalancer.Status = false;
            _globalBalancer.Connection.Close();
        }

        ~GlobalBalancer()
        {
            Dispose();
        }
    }
}
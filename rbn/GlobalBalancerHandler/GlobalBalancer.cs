using System;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common.Logger;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using rbn.Config;
using rbn.Interfaces;
using rbn.QueueHandler.Data;

namespace rbn.GlobalBalancerHandler
{
    /// <summary>
    ///     Делегат передачи результата вычисления веса очереди
    /// </summary>
    /// <returns></returns>
    internal delegate double QueryWeightCompute();

    internal class GlobalBalancer : IServer, IDisposable
    {
        private readonly Data.GlobalBalancer _globalBalancer;
        private readonly PacketTransmitHelper _transmitHelper;

        public GlobalBalancer()
        {
            _transmitHelper = new PacketTransmitHelper();
            _globalBalancer = new Data.GlobalBalancer
            {
                Connection = null,
                Status = true,
                StatusRecived = false
            };
            var globalBalancersThread = new Thread(GlobalBalancerListenThread);
            globalBalancersThread.Start(_globalBalancer);
        }

        public void Dispose()
        {
            _globalBalancer.Status = false;
            _globalBalancer.Connection.Close();
        }

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
                if (
                    !_transmitHelper.Send(queueEntity.RequestPacket.GetPacket(),
                        globalBalancer.Connection.GetStream()))
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

        /// <summary>
        ///     Событие получения запоса
        /// </summary>
        public event Action<Client> RequestRecivedEvent;

        /// <summary>
        ///     Событие подсчета веса
        /// </summary>
        public event QueryWeightCompute QueryWeightComputeEvent;
        
        private void GlobalBalancerListenThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer) param;
            var connection = new TcpClient();

            while (globalBalancer.Status)
            {
                try
                {
                    connection.Connect(
                        RBNConfig.Instance.MRBN.Host,
                        (int) RBNConfig.Instance.MRBN.Port);
                }
                catch (Exception)
                {
                    Logger.Write("Не удалось подключиться к межрегиональному балансировщику. Переподключение");
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.Write("Подключение установлено");

                globalBalancer.Connection = connection;

                var statusSendThread = new Thread(StatusSendThread);
                statusSendThread.Start(globalBalancer);

                while (connection.Connected)
                {
                    Packet packet = _transmitHelper.Recive(connection.GetStream());
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
                            case PacketType.Request:
                                var request = new DbRequestPacket(packet.Data);
                                var client = new Client
                                {
                                    Connection = connection,
                                    RequestPacketData = packet.Data,
                                    DisposeAfterTransmitAnswer = true,
                                    OldId = (int)request.ClientId
                                };
                                if (RequestRecivedEvent != null)
                                    RequestRecivedEvent(client);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("GlobalBalancer. Получен пакет - " + packet.Type);
                        }
                    }
                }
                connection.Close();
                connection = new TcpClient();
            }
        }

        private void StatusSendThread(object param)
        {
            var globalBalancer = (Data.GlobalBalancer) param;
            while (globalBalancer.Connection.Connected)
            {
                if (QueryWeightComputeEvent != null)
                {
                    var packet = new RBNStatusPacket(QueryWeightComputeEvent())
                    {
                        RegionId = RBNConfig.Instance.RBN.RegionId
                    };
                    _transmitHelper.Send(packet.GetPacket(), globalBalancer.Connection.GetStream());
                }
                else
                {
                    return;
                }
                Thread.Sleep(100);
            }
        }

        ~GlobalBalancer()
        {
            Dispose();
        }
    }
}
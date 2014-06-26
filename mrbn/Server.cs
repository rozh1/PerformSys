using System.Net;
using System.Net.Sockets;
using System.Threading;
using Balancer.Common;
using Balancer.Common.Packet;
using Balancer.Common.Packet.Packets;
using Balancer.Common.Utils;
using mrbn.GlobalBalancer.Data;

namespace mrbn
{
    /// <summary>
    ///     МРБН
    /// </summary>
    internal class Server
    {
        /// <summary>
        ///     Балансировщик между регионами
        /// </summary>
        private readonly GlobalBalancer.GlobalBalancer _balancer;

        /// <summary>
        ///     Слушатель новых соединений (клиентов)
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        ///     признак жизн потока
        /// </summary>
        private bool _serverIsLife;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            Logger.Write("Начато прослушивание " + IPAddress.Any + ":" + port);

            _balancer = new GlobalBalancer.GlobalBalancer();

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
        ///     Поток обслуживания клиентского соедиения
        /// </summary>
        /// <param name="param"></param>
        private void ClientThread(object param)
        {
            var rbnClient = (TcpClient) param;

            Packet statusPacket = PacketTransmitHelper.Recive(rbnClient.GetStream());
            if (statusPacket == null) return;
            if (statusPacket.Type != PacketType.RBNStatus) return;

            var rbnStatusPacket = new RBNStatusPacket(statusPacket.Data);
            var rbn = new RBN
            {
                RbnClient = rbnClient,
                RegionId = (int) rbnStatusPacket.RegionId
            };

            if (!_balancer.AddRbn(rbn))
            {
                rbnClient.Close();
                return;
            }

            while (rbnClient.Connected)
            {
                Packet packet = PacketTransmitHelper.Recive(rbnClient.GetStream());
                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.RBNStatus:
                            break;
                    }
                }
            }

            _balancer.Remove(rbn);
            rbnClient.Close();
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
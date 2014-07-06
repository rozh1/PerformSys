using Balancer.Common.Packet.Packets;

namespace rbn.QueueHandler.Data
{
    public class QueueEntity
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public int ClientId {
            get { return (int)RequestPacket.ClientId; } 
        }

        /// <summary>
        /// Пакет запроса к БД
        /// </summary>
        public DbRequestPacket RequestPacket { get; set; }

        /// <summary>
        /// Объем отношений для обработки запроса
        /// </summary>
        public double relationVolume { get; set; }
    }
}
using PerformSys.Common.Packet.Packets;

namespace rbn.QueueHandler.Data
{
    public class QueueEntity
    {
        /// <summary>
        /// Идентификатор клиента
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Пакет запроса к БД
        /// </summary>
        public DbRequestPacket RequestPacket { get; set; }

        /// <summary>
        /// Объем отношений для обработки запроса
        /// </summary>
        public double RelationVolume { get; set; }
    }
}
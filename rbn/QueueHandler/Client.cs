using System.Net.Sockets;

namespace rbn.QueueHandler
{
    internal class Client
    {
        public TcpClient Connection { get; set; }
        public string RequestPacketData { get; set; }
        public string AnswerPacketData { get; set; }
        public bool RequestSended { get; set; }
        public int Id { get; set; }
    }
}
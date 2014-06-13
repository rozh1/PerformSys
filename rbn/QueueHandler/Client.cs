using System.Net.Sockets;

namespace rbn.QueueHandler
{
    internal class Client
    {
        public TcpClient Connection { get; set; }
        public string Query { get; set; }
        public string AnswerPacketData { get; set; }
        public bool QuerySended { get; set; }
        public int Id { get; set; }
    }
}
using System.Net.Sockets;

namespace rbn.QueueHandler.Data
{
    public class Client
    {
        public TcpClient Connection { get; set; }
        public string RequestPacketData { get; set; }
        public string AnswerPacketData { get; set; }
        public bool RequestSended { get; set; }
        public int Id { get; set; }
        public bool DisposeAfterTransmitAnswer { get; set; }
    }
}
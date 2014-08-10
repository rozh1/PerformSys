using System;
using System.Net.Sockets;
using rbn.Config.Data;

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
        public LogStats LogStats { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime SendedTime { get; set; }
    }
}
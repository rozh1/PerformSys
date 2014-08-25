using System;
using System.Net.Sockets;
using rbn.Config.Data;
using rbn.Config.Data.LogData;

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
        public Stats LogStats { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime SendedTime { get; set; }
    }
}
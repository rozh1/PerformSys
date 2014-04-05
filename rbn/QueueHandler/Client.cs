﻿using System.Net.Sockets;

namespace rbn.QueueHandler
{
    class Client
    {
        public TcpClient Connection { get; set; }
        public string Query { get; set; }
        public string AnswerPacketData { get; set; }
        public bool QuerySended { get; set; }
    }
}

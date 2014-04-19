using System.Net.Sockets;

namespace rbn.ServersHandler
{
    class Server
    {
        public TcpClient Connection { get; set; }
        public bool Status { get; set; }
        public bool StatusRecived { get; set; }
    }
}

using System.Net.Sockets;

namespace rbn.GlobalBalancerHandler
{
    internal class GlobalBalancer
    {
        public TcpClient Connection { get; set; }
        public bool Status { get; set; }
        public bool StatusRecived { get; set; }
    }
}
using System.Net.Sockets;

namespace rbn.GlobalBalancerHandler.Data
{
    public class GlobalBalancer
    {
        public TcpClient Connection { get; set; }
        public bool Status { get; set; }
        public bool StatusRecived { get; set; }
    }
}
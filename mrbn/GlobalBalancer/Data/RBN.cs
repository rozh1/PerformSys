using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace mrbn.GlobalBalancer.Data
{
    class RBN
    {
        public TcpClient RbnClient { get; set; }
        public uint RegionId { get; set; }
        public uint GlobalId { get; set; }
        public double Weight { get; set; }
        public RBN RelayRbn { get; set; }
    }
}

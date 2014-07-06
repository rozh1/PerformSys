using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rbn.Config.Data
{
    public class RBN
    {
        public uint GlobalId { get; set; }
        public uint RegionId { get; set; }
        public uint Port { get; set; }
        public int ServersCount { get; set; }
        public int MaxServersCount { get; set; }
    }
}

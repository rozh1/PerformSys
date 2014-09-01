using Balancer.Common.Config;
using mrbn.Config.Data;

namespace mrbn.Config
{
    public class MRBNConfig : ConfigBase<MRBNConfig>
    {
        public MRBN MRBN { get; set; }

        public Log Log { get; set; }
    }
}
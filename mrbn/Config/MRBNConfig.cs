using mrbn.Config.Data;
using PerformSys.Common.Config;

namespace mrbn.Config
{
    public class MRBNConfig : ConfigBase<MRBNConfig>
    {
        public MRBN MRBN { get; set; }

        public Log Log { get; set; }
    }
}
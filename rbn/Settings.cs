using System.IO;
using Balancer.Common;
using rbn.ServersHandler;

namespace rbn
{
    static class Settings
    {
        static public int GlobalId { get; set; }
        static public int RegionId { get; set; }

        public static void Init()
        {
            GlobalId = int.Parse(ConfigFile.GetConfigValue("Global_id"));

            RegionId = int.Parse(ConfigFile.GetConfigValue("Region_id"));
        }
    }
}
using Balancer.Common;

namespace rbn
{
    internal static class Settings
    {
        public static int GlobalId { get; set; }
        public static int RegionId { get; set; }

        public static void Init()
        {
            GlobalId = int.Parse(ConfigFile.GetConfigValue("Global_id"));

            RegionId = int.Parse(ConfigFile.GetConfigValue("Region_id"));
        }
    }
}
using System.Collections.Generic;

namespace rbn.QueueHandler.Data
{
    class TableSizes
    {
        public int RegionId { get; set; }
        public int GlobalId { get; set; }
        public Dictionary<string, uint> Sizes { get; set; } 
    }
}

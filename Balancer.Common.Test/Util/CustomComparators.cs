using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balancer.Common.Test.Util
{
    class CustomComparators
    {
        public static bool AreTablesEqual(DataTable t1, DataTable t2)
        {
            if (t1.Rows.Count != t2.Rows.Count)
                return false;

            for (int i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Columns.Cast<DataColumn>().Any(col => !Equals(t1.Rows[i][col.ColumnName], t2.Rows[i][col.ColumnName])))
                {
                    return false;
                }
            }
            return true;
        }

    }
}

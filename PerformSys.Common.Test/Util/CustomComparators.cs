#region Copyright
/*
 * Copyright 2013-2018 Roman Klassen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */
 #endregion
using System.Data;
using System.Linq;

namespace PerformSys.Common.Test.Util
{
    internal static class CustomComparators
    {
        public static bool AreTablesEqual(DataTable t1, DataTable t2)
        {
            if (t1.Rows.Count != t2.Rows.Count)
                return false;

            for (int i = 0; i < t1.Rows.Count; i++)
            {
                if (
                    t1.Columns.Cast<DataColumn>()
                        .Any(col => !Equals(t1.Rows[i][col.ColumnName], t2.Rows[i][col.ColumnName])))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
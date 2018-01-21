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

﻿using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PerformSys.Common.Test.Util
{
    [TestClass]
    public class CustomComparatorsTest
    {
        [TestMethod]
        public void AreTablesEqualSameDataTableTest()
        {
            var dt = new DataTable();
            dt.TableName = "randomTable";
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            var dt2 = new DataTable();
            dt2.TableName = "randomTable";
            dt2.Columns.Add("Data");
            DataRow dr2 = dt2.NewRow();
            dr2[0] = "RandomData";
            dt2.Rows.Add(dr2);
            Assert.IsTrue(CustomComparators.AreTablesEqual(dt, dt2));
        }

        [TestMethod]
        public void AreTablesEqualDifferentDataTableTest()
        {
            var dt = new DataTable();
            dt.TableName = "randomTable";
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            var dt2 = new DataTable();
            dt2.TableName = "diffTable";
            dt2.Columns.Add("Data");
            DataRow dr2 = dt2.NewRow();
            dr2[0] = "diffData";
            dt2.Rows.Add(dr2);
            Assert.IsFalse(CustomComparators.AreTablesEqual(dt, dt2));
        }

        [TestMethod]
        public void AreTablesEqualDifferentDataTableLengthTest()
        {
            var dt = new DataTable();
            dt.TableName = "randomTable";
            dt.Columns.Add("Data");
            DataRow dr = dt.NewRow();
            dr[0] = "RandomData";
            dt.Rows.Add(dr);
            var dt2 = new DataTable();
            dt2.TableName = "diffTable";
            Assert.IsFalse(CustomComparators.AreTablesEqual(dt, dt2));
        }
    }
}
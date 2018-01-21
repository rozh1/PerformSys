using System.Data;
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
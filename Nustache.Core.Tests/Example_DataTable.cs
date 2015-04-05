using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Example_DataTable
    {
        [Test]
        public void It_can_Render_Datatables()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("IntColumn", typeof(int));
            dt.Rows.Add(new object[] { 1 });
            dt.Rows.Add(new object[] { 2 });
            dt.Rows.Add(new object[] { 3 });

            var result = Render.StringToString("{{#Item}}{{IntColumn}}{{/Item}}", new { Item = dt });

            Assert.AreEqual("123", result);
        }
    }
}

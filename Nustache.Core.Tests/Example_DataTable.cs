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

        [Test]
        public void It_can_Render_Datatables_Case_Insensitive()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("IntColumn", typeof(int));
            dt.Rows.Add(new object[] { 1 });
            dt.Rows.Add(new object[] { 2 });
            dt.Rows.Add(new object[] { 3 });

            var result = Render.StringToString("{{#Item}}{{intcolumn}}{{/Item}}", new { Item = dt });

            Assert.AreEqual("123", result);
        }

        [Test]
        public void It_Should_Render_Inverted_When_Having_No_Rows()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add("Foo");

            var result = Render.StringToString(
                @"<table>
                    <thead>
                        <tr>
                            <th>Foo</th>
                        </tr>
                    </thead>
                    <tbody>
                        {{#Data}}
                        <tr>
                            <td>{{Foo}}</td>
                        </tr>
                        {{/Data}}
                        {{^Data}}
                        <tr>
                            <td>
                                No data exists.
                            </td>
                        </tr>
                        {{/Data}}
                    </tbody>
                </table>", new { Data = dt });

            Assert.AreEqual(
                @"<table>
                    <thead>
                        <tr>
                            <th>Foo</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                No data exists.
                            </td>
                        </tr>
                    </tbody>
                </table>", result);
        }
    }
}

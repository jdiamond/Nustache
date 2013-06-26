using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Method
    {
        [Test]
        public void It_can_render_method_result()
        {
            var result = Render.StringToString("{{#Rows}}{{#Columns}}{{GetValue}}{{/Columns}}{{/Rows}}", GetTableData());
            Assert.AreEqual("tttttt", result);
        }
                
        [Test]
        public void It_can_render_method_result_with_context()
        {
            var result = Render.StringToString("{{#Rows}}{{#Columns}}{{GetValueWithContext}}{{/Columns}}{{/Rows}}", GetTableData());
            Assert.AreEqual("123246", result);
        }
                
        private static TableData GetTableData()
        {
            return new TableData()
            {
                Rows = new int[] { 1, 2 },
                Columns = new int[] { 1, 2, 3 },
            };
        }
    }

    class TableData
    {
        public int[] Rows { get; set; }
        public int[] Columns { get; set; }

        public string GetValue()
        {
            return "t";
        }

        public string GetValueWithContext(RenderContext renderContext)
        {
            int row = (int)renderContext.GetSectionData("Rows");
            int column = (int)renderContext.GetSectionData("Columns");
            return (row * column).ToString();
        }
    }
}

using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Lambda
    {
        [Test]
        public void The_text_passed_is_the_literal_block_unrendered()
        {
            var result = Render.StringToString("{{#wrapped}}{{name}} is awesome.{{/wrapped}}", new
            {
                wrapped = (Lambda)((s) => string.Format("<b>{0}</b>", s))
            });
            Assert.AreEqual("<b>{{name}} is awesome.</b>", result);
        }
    }
}
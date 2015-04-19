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
                wrapped = (Lambda<string, string>)((body) => string.Format("<b>{0}</b>", body)),
                name = "Nustache"
            });
            Assert.AreEqual("<b>Nustache is awesome.</b>", result);
        }

        [Test]
        public void Lambdas_Return_Should_Be_Interpolated()
        {
            var result = Render.StringToString("Hello, {{lambda}}!", new
            {
                lambda = (Lambda<string>)(() => "World")
            });

            Assert.AreEqual("Hello, World!", result);
        }
    }
}
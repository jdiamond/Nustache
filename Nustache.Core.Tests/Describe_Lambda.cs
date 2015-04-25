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
                wrapped = (Lambda<string, object>)((body) => string.Format("<b>{0}</b>", body)),
                name = "Nustache"
            });
            Assert.AreEqual("<b>Nustache is awesome.</b>", result);
        }

        [Test]
        public void It_can_render_nontyped_delegate_functions()
        {
            var result = Render.StringToString("{{foo}}, {{bar}}", new
            {
                foo = "bar",
                bar = new System.Func<string>(() => { return "foo"; })
            });

            Assert.AreEqual("bar, foo", result);
        }

        [Test]
        public void It_can_render_nontyped_delegate_functions_with_body()
        {
            var result = Render.StringToString("{{#wrapped}}{{name}} is awesome.{{/wrapped}}", new
            {
                wrapped = new System.Func<string, string>((body) => string.Format("<b>{0}</b>", body)),
                name = "Nustache"
            });
            Assert.AreEqual("<b>Nustache is awesome.</b>", result);
        }
    }
}
using System;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_TemplateDefinition
    {
        [Test]
        public void It_cant_be_constructed_with_a_null_name()
        {
            Assert.Throws<ArgumentNullException>(() => new TemplateDefinition(null));
        }

        [Test]
        public void It_renders_its_child_parts()
        {
            var a = new TemplateDefinition("a");
            a.Load(new Part[] { new LiteralText("b") });
            var writer = new StringWriter();
            var context = new RenderContext(new Template(), null, writer, null);

            a.Render(context);

            Assert.AreEqual("b", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new TemplateDefinition("a");

            Assert.AreEqual("TemplateDefinition(\"a\")", a.ToString());
        }
    }
}
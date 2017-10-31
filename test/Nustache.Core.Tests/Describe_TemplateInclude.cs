using System;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_TemplateInclude
    {
        [Test]
        public void It_cant_be_constructed_with_a_null_name()
        {
            Assert.Throws<ArgumentNullException>(() => new TemplateInclude(null));
        }

        [Test]
        public void It_asks_the_RenderContext_to_include_a_named_template()
        {
            var a = new TemplateInclude("a");
            var template = new Template();
            template.Load(new Part[] { new LiteralText("b") });
            var writer = new StringWriter();
            var context = new RenderContext(new Template(), null, writer, name => template);

            a.Render(context);

            Assert.AreEqual("b", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new TemplateInclude("a");

            Assert.AreEqual("TemplateInclude(\"a\")", a.ToString());
        }

        [Test]
        public void It_trims_spaces()
        {
            var a = new TemplateInclude(" a ");
            Assert.AreEqual(a.Name, "a");
        }
    }
}
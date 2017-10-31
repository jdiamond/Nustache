using System;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_LiteralText
    {
        [Test]
        public void It_cant_be_constructed_with_null_text()
        {
            Assert.Throws<ArgumentNullException>(() => new LiteralText(null));
        }

        [Test]
        public void It_renders_its_text()
        {
            var a = new LiteralText("a");
            var writer = new StringWriter();
            var context = new RenderContext(null, null, writer, null);

            a.Render(context);

            Assert.AreEqual("a", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new LiteralText("a");

            Assert.AreEqual("LiteralText(\"a\")", a.ToString());
        }
    }
}
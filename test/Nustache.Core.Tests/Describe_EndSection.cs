using System;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_EndSection
    {
        [Test]
        public void It_cant_be_constructed_with_a_null_name()
        {
            Assert.Throws<ArgumentNullException>(() => new EndSection(null));
        }

        [Test]
        public void It_renders_nothing()
        {
            var a = new EndSection("a");
            var writer = new StringWriter();
            var context = new RenderContext(null, null, writer, null);

            a.Render(context);

            Assert.AreEqual("", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new EndSection("a");

            Assert.AreEqual("EndSection(\"a\")", a.ToString());
        }
    }
}
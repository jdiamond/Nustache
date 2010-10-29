using System;
using System.IO;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
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
        public void It_has_a_useful_Equals_method()
        {
            object a = new LiteralText("a");
            object a2 = new LiteralText("a");
            object b = new LiteralText("b");

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(a2));
            Assert.IsTrue(a2.Equals(a));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a.Equals("a"));
        }

        [Test]
        public void It_has_an_Equals_overload_for_other_LiteralText_objects()
        {
            var a = new LiteralText("a");
            var a2 = new LiteralText("a");
            var b = new LiteralText("b");

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(a2));
            Assert.IsTrue(a2.Equals(a));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
            Assert.IsFalse(a.Equals(null));
        }

        [Test]
        public void It_has_a_useful_GetHashCode_method()
        {
            var a = new LiteralText("a");

            Assert.AreNotEqual(0, a.GetHashCode());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new LiteralText("a");

            Assert.AreEqual("LiteralText(\"a\")", a.ToString());
        }
    }
}
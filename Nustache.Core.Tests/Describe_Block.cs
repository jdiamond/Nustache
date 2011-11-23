using System;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Block
    {
        [Test]
        public void It_cant_be_constructed_with_a_null_name()
        {
            Assert.Throws<ArgumentNullException>(() => new Block(null));
        }

        [Test]
        public void It_renders_its_child_parts_if_name_evaluates_to_true()
        {
            var a = new Block("a", new LiteralText("b"));
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = true }, writer, null);

            a.Render(context);

            Assert.AreEqual("b", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_does_not_render_its_child_parts_if_name_evaluates_to_empty_string()
        {
            var a = new Block("a", new LiteralText("b"));
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = "" }, writer, null);

            a.Render(context);

            Assert.AreEqual("", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_renders_its_child_parts_once_for_each_item_if_name_evaluates_to_a_collection()
        {
            var a = new Block("a", new LiteralText("b"));
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = new [] { 1, 2, 3 } }, writer, null);

            a.Render(context);

            Assert.AreEqual("bbb", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new Block("a");

            Assert.AreEqual("Block(\"a\")", a.ToString());
        }
    }
}
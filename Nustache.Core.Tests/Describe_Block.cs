using System;
using System.Collections.Generic;
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
            var context = new RenderContext(null, new { a = true }, writer, null, Options.Defaults());

            a.Render(context);

            Assert.AreEqual("b", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_does_not_render_its_child_parts_if_name_evaluates_to_empty_string()
        {
            var a = new Block("a", new LiteralText("b"));
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = "" }, writer, null, Options.Defaults());

            a.Render(context);

            Assert.AreEqual("", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_renders_its_child_parts_once_for_each_item_if_name_evaluates_to_a_collection()
        {
            var a = new Block("a", new LiteralText("b"));
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = new[] { 1, 2, 3 } }, writer, null, Options.Defaults());

            a.Render(context);

            Assert.AreEqual("bbb", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_supports_accessing_members_of_child_objects()
        {
            var a = new Block("a.b", new LiteralText("c"));
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = new { b = new[] { 1, 2, 3 } } }, writer, null, Options.Defaults());

            a.Render(context);

            Assert.AreEqual("ccc", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_does_not_treat_dictionaries_as_lists()
        {
            var a = new Block("a", new LiteralText("x"));
            var writer = new StringWriter();
            var context = new RenderContext(
                null,
                new { a = new Dictionary<string, int> { { "b", 1 }, { "c", 2 }, { "d", 3 } } },
                writer,
                null,
                Options.Defaults());

            a.Render(context);

            Assert.AreEqual("x", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new Block("a");

            Assert.AreEqual("Block(\"a\")", a.ToString());
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_InvertedBlock
    {
        [Test]
        public void It_cant_be_constructed_with_a_null_name()
        {
            Assert.Throws<ArgumentNullException>(() => new InvertedBlock(null));
        }

        [Test]
        public void It_doesnt_render_its_child_parts_if_name_evaluates_to_true()
        {
            AssertDoesntRender(true);
        }

        [Test]
        public void It_renders_its_child_parts_if_name_evaluates_to_false()
        {
            AssertRender(false);
        }

        [Test]
        public void It_doesnt_render_its_child_parts_if_name_evaluates_to_populated_collection()
        {
            AssertDoesntRender(new[] { "z", "x", "y" });
        }

        [Test]
        public void It_renders_its_child_parts_if_name_evaluates_to_empty_collection()
        {
            AssertRender(new List<string>());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new InvertedBlock("a");

            Assert.AreEqual("InvertedBlock(\"a\")", a.ToString());
        }

        private static void AssertRender<T>(T value)
        {
            AssertRenderingResult(Assert.AreEqual, value);
        }

        private static void AssertDoesntRender<T>(T value)
        {
            AssertRenderingResult((text, result) => Assert.IsEmpty(result), value);
        }

        private static void AssertRenderingResult<T>(Action<string, string> assertion, T value)
        {
            var text = new LiteralText("b");
            var inverted = new InvertedBlock("a", text);

            using (var writer = new StringWriter()) {
                var context = new RenderContext(null, new { a = value }, writer, null);

                inverted.Render(context);

                assertion(text.Text, writer.GetStringBuilder().ToString());
            }
        }
    }
}

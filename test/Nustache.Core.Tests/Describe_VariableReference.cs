using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_VariableReference
    {
        [Test]
        public void It_cant_be_constructed_with_a_null_name()
        {
            Assert.Throws<ArgumentNullException>(() => new VariableReference(null));
        }

        [Test]
        public void It_renders_the_named_value_from_the_context()
        {
            var a = new VariableReference("a");
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = "b" }, writer, null);

            a.Render(context);

            Assert.AreEqual("b", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_supports_accessing_members_of_child_objects()
        {
            var a = new VariableReference("a.b");
            var writer = new StringWriter();
            var context = new RenderContext(null, new { a = new { b = "c" } }, writer, null);

            a.Render(context);

            Assert.AreEqual("c", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_checks_for_keys_containing_dots_before_splitting()
        {
            var a = new VariableReference("a.b");
            var writer = new StringWriter();
            var context = new RenderContext(null, new Dictionary<string, string> { { "a.b", "c" } }, writer, null);

            a.Render(context);

            Assert.AreEqual("c", writer.GetStringBuilder().ToString());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new VariableReference("a");

            Assert.AreEqual("VariableReference(\"a\")", a.ToString());
        }
    }
}
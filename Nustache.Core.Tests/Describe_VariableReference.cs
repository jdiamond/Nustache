using System;
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
        public void It_has_a_useful_ToString_method()
        {
            var a = new VariableReference("a");

            Assert.AreEqual("VariableReference(\"a\")", a.ToString());
        }
    }
}
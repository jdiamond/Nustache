using System;
using System.IO;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
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
        public void It_has_a_useful_Equals_method()
        {
            object a = new VariableReference("a");
            object a2 = new VariableReference("a");
            object b = new VariableReference("b");

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(a2));
            Assert.IsTrue(a2.Equals(a));
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a.Equals("a"));
        }

        [Test]
        public void It_has_an_Equals_overload_for_other_VariableReference_objects()
        {
            var a = new VariableReference("a");
            var a2 = new VariableReference("a");
            var b = new VariableReference("b");

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
            var a = new VariableReference("a");

            Assert.AreNotEqual(0, a.GetHashCode());
        }

        [Test]
        public void It_has_a_useful_ToString_method()
        {
            var a = new VariableReference("a");

            Assert.AreEqual("VariableReference(\"a\")", a.ToString());
        }
    }
}
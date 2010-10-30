using System.Linq;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_Scanner
    {
        [Test]
        public void It_returns_no_parts_for_the_empty_string()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }

        [Test]
        public void It_scans_literal_text()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("foo");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                          new LiteralText("foo"),
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }

        [Test]
        public void It_scans_variable_references()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{foo}}after");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                          new LiteralText("before"),
                                          new VariableReference("foo"),
                                          new LiteralText("after"),
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }

        [Test]
        public void It_scans_sections()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("{{#foo}}inside{{/foo}}");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                          new Block("foo"),
                                          new LiteralText("inside"),
                                          new EndSection("foo"),
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }

        public void It_scans_template_definitions()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("{{<foo}}inside{{/foo}}");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                          new TemplateDefinition("foo"),
                                          new LiteralText("inside"),
                                          new EndSection("foo"),
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }

        [Test]
        public void It_scans_template_includes()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{>foo}}after");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                          new LiteralText("before"),
                                          new TemplateInclude("foo"),
                                          new LiteralText("after"),
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }

        [Test]
        public void It_skips_comments()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{!foo}}after");

            CollectionAssert.AreEqual(new Part[]
                                      {
                                          new LiteralText("before"),
                                          new LiteralText("after"),
                                      },
                                      parts.ToArray(),
                                      new PartComparer());
        }
    }
}
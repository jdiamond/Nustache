using System.Linq;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_Parser_Parse
    {
        [Test]
        public void It_combines_sections()
        {
            var parser = new Parser();
            var template = new Template();

            parser.Parse(template,
                         new Part[]
                             {
                                 new LiteralText("before"),
                                 new Block("foo"),
                                 new LiteralText("inside"),
                                 new EndSection("foo"),
                                 new LiteralText("after")
                             });

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("before"),
                        new Block("foo",
                                  new LiteralText("inside"),
                                  new EndSection("foo")),
                        new LiteralText("after")
                    },
                template.Parts.ToArray());
        }

        [Test]
        public void It_handles_nested_sections()
        {
            var parser = new Parser();
            var template = new Template();

            parser.Parse(template,
                         new Part[]
                             {
                                 new LiteralText("before foo"),
                                 new Block("foo"),
                                 new LiteralText("before bar"),
                                 new Block("bar"),
                                 new LiteralText("inside bar"),
                                 new EndSection("bar"),
                                 new LiteralText("after bar"),
                                 new EndSection("foo"),
                                 new LiteralText("after foo")
                             });

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("before foo"),
                        new Block("foo",
                                  new LiteralText("before bar"),
                                  new Block("bar",
                                            new LiteralText("inside bar"),
                                            new EndSection("bar")),
                                  new LiteralText("after bar"),
                                  new EndSection("foo")),
                        new LiteralText("after foo")
                    },
                template.Parts.ToArray());
        }

        [Test]
        public void It_handles_nested_sections_with_the_same_name()
        {
            var parser = new Parser();
            var template = new Template();

            parser.Parse(template,
                         new Part[]
                             {
                                 new LiteralText("before foo 1"),
                                 new Block("foo"),
                                 new LiteralText("before foo 2"),
                                 new Block("foo"),
                                 new LiteralText("inside foo 2"),
                                 new EndSection("foo"),
                                 new LiteralText("after foo 2"),
                                 new EndSection("foo"),
                                 new LiteralText("after foo 1")
                             });

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("before foo 1"),
                        new Block("foo",
                                  new LiteralText("before foo 2"),
                                  new Block("foo",
                                            new LiteralText("inside foo 2"),
                                            new EndSection("foo")),
                                  new LiteralText("after foo 2"),
                                  new EndSection("foo")),
                        new LiteralText("after foo 1")
                    },
                template.Parts.ToArray());
        }

        [Test]
        public void It_throws_when_the_end_section_does_not_match_the_start_section()
        {
            var parser = new Parser();
            var template = new Template();

            Assert.Catch<NustacheException>(
                () => parser.Parse(template,
                                   new Part[]
                                       {
                                           new LiteralText("before"),
                                           new Block("foo"),
                                           new LiteralText("inside"),
                                           new EndSection("bar"),
                                           new LiteralText("after")
                                       }));
        }

        [Test]
        public void It_throws_when_the_end_section_does_not_match_any_start_section()
        {
            var parser = new Parser();
            var template = new Template();

            Assert.Catch<NustacheException>(
                () => parser.Parse(template,
                                   new Part[]
                                       {
                                           new LiteralText("before"),
                                           new EndSection("foo"),
                                           new LiteralText("after")
                                       }));
        }
    }
}
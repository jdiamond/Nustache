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

            var parts = parser.Parse(
                new Part[]
                    {
                        new LiteralText("before"),
                        new StartSection("foo"),
                        new LiteralText("inside"),
                        new EndSection("foo"),
                        new LiteralText("after"),
                    });

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("before"),
                        new StartSection("foo")
                            {
                                Children =
                                    {
                                        new LiteralText("inside"),
                                        new EndSection("foo"),
                                    }
                            },
                        new LiteralText("after"),
                    },
                parts.ToArray());
        }

        [Test]
        public void It_handles_nested_sections()
        {
            var parser = new Parser();

            var parts = parser.Parse(
                new Part[]
                    {
                        new LiteralText("before foo"),
                        new StartSection("foo"),
                        new LiteralText("before bar"),
                        new StartSection("bar"),
                        new LiteralText("inside bar"),
                        new StartSection("bar"),
                        new LiteralText("after bar"),
                        new EndSection("foo"),
                        new LiteralText("after foo"),
                    });

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("before foo"),
                        new StartSection("foo")
                            {
                                Children =
                                    {
                                        new LiteralText("before bar"),
                                        new StartSection("bar")
                                            {
                                                Children =
                                                    {
                                                        new LiteralText("inside bar"),
                                                        new EndSection("bar"),
                                                    }
                                            },
                                        new LiteralText("after bar"),
                                        new EndSection("foo"),
                                    }
                            },
                        new LiteralText("after foo"),
                    },
                parts.ToArray());
        }

        [Test]
        public void It_handles_nested_sections_with_the_same_name()
        {
            var parser = new Parser();

            var parts = parser.Parse(
                new Part[]
                    {
                        new LiteralText("before foo 1"),
                        new StartSection("foo"),
                        new LiteralText("before foo 2"),
                        new StartSection("foo"),
                        new LiteralText("inside foo 2"),
                        new StartSection("foo"),
                        new LiteralText("after foo 2"),
                        new EndSection("foo"),
                        new LiteralText("after foo 1"),
                    });

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("before foo 1"),
                        new StartSection("foo")
                            {
                                Children =
                                    {
                                        new LiteralText("before foo 2"),
                                        new StartSection("foo")
                                            {
                                                Children =
                                                    {
                                                        new LiteralText("inside foo 2"),
                                                        new EndSection("foo"),
                                                    }
                                            },
                                        new LiteralText("after foo 2"),
                                        new EndSection("foo"),
                                    }
                            },
                        new LiteralText("after foo 1"),
                    },
                parts.ToArray());
        }

        [Test]
        public void It_throws_when_the_end_section_does_not_match_the_start_section()
        {
            var parser = new Parser();

            Assert.Catch<NustacheException>(() => parser.Parse(
                                                      new Part[]
                                                          {
                                                              new LiteralText("before"),
                                                              new StartSection("foo"),
                                                              new LiteralText("inside"),
                                                              new EndSection("bar"),
                                                              new LiteralText("after"),
                                                          })
                                                      .ToArray());
        }
    }
}
using System;
using System.Linq;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Parser
    {
        [Test]
        public void It_throws_when_section_is_null()
        {
            var parser = new Parser();

            Assert.Throws<ArgumentNullException>(() => parser.Parse(null, null));
        }

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

            template.Parts.IsEqualTo(new LiteralText("before"),
                                     new Block("foo",
                                               new LiteralText("inside"),
                                               new EndSection("foo")),
                                     new LiteralText("after"));
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

            template.Parts.IsEqualTo(new LiteralText("before foo"),
                                     new Block("foo",
                                               new LiteralText("before bar"),
                                               new Block("bar",
                                                         new LiteralText("inside bar"),
                                                         new EndSection("bar")),
                                               new LiteralText("after bar"),
                                               new EndSection("foo")),
                                     new LiteralText("after foo"));
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

            template.Parts.IsEqualTo(new LiteralText("before foo 1"),
                                     new Block("foo",
                                               new LiteralText("before foo 2"),
                                               new Block("foo",
                                                         new LiteralText("inside foo 2"),
                                                         new EndSection("foo")),
                                               new LiteralText("after foo 2"),
                                               new EndSection("foo")),
                                     new LiteralText("after foo 1"));
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

        [Test]
        public void It_treats_sections_named_else_as_inverse_sections()
        {
            var parser = new Parser();
            var template = new Template();

            parser.Parse(template,
                         new Part[]
                             {
                                 new LiteralText("before"),
                                 new Block("foo"),
                                 new LiteralText("inside1"),
                                 new Block("else"),
                                 new LiteralText("inside2"), 
                                 new EndSection("foo"),
                                 new LiteralText("after")
                             });

            template.Parts.IsEqualTo(new LiteralText("before"),
                                     new Block("foo",
                                               new LiteralText("inside1"),
                                               new EndSection("foo")),
                                     new LiteralText("after"));

            ((Block)template.Parts.ToArray()[1]).Inverse.IsEqualTo(new Block("else", new LiteralText("inside2")));
        }
    }
}

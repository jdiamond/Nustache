using System.Linq;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_Section
    {
        [Test]
        public void It_holds_parts_added_to_it()
        {
            var section = new Section("foo");

            section.Add(new LiteralText("bar"));
            section.Add(new LiteralText("baz"));

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("bar"),
                        new LiteralText("baz")
                    },
                section.Parts.ToArray());
        }

        [Test]
        public void It_does_not_hold_template_definitions_with_other_parts()
        {
            var section = new Section("foo");

            section.Add(new LiteralText("bar"));
            section.Add(new TemplateDefinition("baz"));
            section.Add(new LiteralText("quux"));

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("bar"),
                        new LiteralText("quux")
                    },
                section.Parts.ToArray());
        }

        [Test]
        public void It_allows_you_to_look_up_template_definitions_by_name()
        {
            var section = new Section("foo");
            var templateDefinition = new TemplateDefinition("bar");
            section.Add(templateDefinition);

            var actual = section.GetTemplateDefinition(templateDefinition.Name);

            Assert.AreSame(templateDefinition, actual);
        }
    }
}
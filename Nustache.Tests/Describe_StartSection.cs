using System.Linq;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_StartSection
    {
        [Test]
        public void It_holds_parts_added_to_it()
        {
            var startSection = new StartSection("foo");

            startSection.Add(new LiteralText("bar"));
            startSection.Add(new LiteralText("baz"));

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("bar"),
                        new LiteralText("baz")
                    },
                startSection.Children.ToArray());
        }

        [Test]
        public void It_does_not_treat_template_definitions_as_children()
        {
            var startSection = new StartSection("foo");

            startSection.Add(new LiteralText("bar"));
            startSection.Add(new TemplateDefinition("baz"));
            startSection.Add(new LiteralText("quux"));

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("bar"),
                        new LiteralText("quux")
                    },
                startSection.Children.ToArray());
        }

        [Test]
        public void It_allows_you_to_look_up_template_definitions_by_name()
        {
            var startSection = new StartSection("foo");
            var templateDefinition = new TemplateDefinition("bar");
            startSection.Add(templateDefinition);

            var actual = startSection.GetTemplateDefinition(templateDefinition.Name);

            Assert.AreSame(templateDefinition, actual);
        }
    }
}
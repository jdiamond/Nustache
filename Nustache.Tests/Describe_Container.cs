using System.Linq;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_Container
    {
        [Test]
        public void It_holds_parts_added_to_it()
        {
            var container = new Container("foo");

            container.Add(new LiteralText("bar"));
            container.Add(new LiteralText("baz"));

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("bar"),
                        new LiteralText("baz")
                    },
                container.Children.ToArray());
        }

        [Test]
        public void It_does_not_treat_template_definitions_as_children()
        {
            var container = new Container("foo");

            container.Add(new LiteralText("bar"));
            container.Add(new TemplateDefinition("baz"));
            container.Add(new LiteralText("quux"));

            CollectionAssert.AreEqual(
                new Part[]
                    {
                        new LiteralText("bar"),
                        new LiteralText("quux")
                    },
                container.Children.ToArray());
        }

        [Test]
        public void It_allows_you_to_look_up_template_definitions_by_name()
        {
            var container = new Container("foo");
            var templateDefinition = new TemplateDefinition("bar");
            container.Add(templateDefinition);

            var actual = container.GetTemplateDefinition(templateDefinition.Name);

            Assert.AreSame(templateDefinition, actual);
        }
    }
}
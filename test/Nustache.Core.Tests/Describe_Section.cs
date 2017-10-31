using NUnit.Framework;

namespace Nustache.Core.Tests
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

            section.Parts.IsEqualTo(new LiteralText("bar"),
                                    new LiteralText("baz"));
        }

        [Test]
        public void It_does_not_hold_template_definitions_with_other_parts()
        {
            var section = new Section("foo");

            section.Add(new LiteralText("bar"));
            section.Add(new TemplateDefinition("baz"));
            section.Add(new LiteralText("quux"));

            section.Parts.IsEqualTo(new LiteralText("bar"),
                                    new LiteralText("quux"));
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
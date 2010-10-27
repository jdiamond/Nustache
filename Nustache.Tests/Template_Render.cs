using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_Template_Render
    {
        [Test]
        public void It_renders_empty_strings_as_empty_strings()
        {
            var result = Template.RenderStringToString("", null);
            Assert.AreEqual("", result);
        }

        [Test]
        public void It_replaces_undefined_variables_with_empty_strings_when_there_is_no_data()
        {
            var result = Template.RenderStringToString("before{{foo}}after", null);
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_replaces_undefined_variables_with_empty_strings_when_there_is_data()
        {
            var result = Template.RenderStringToString("before{{foo}}after", new { bar = "baz" });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_replaces_defined_variables_with_values()
        {
            var result = Template.RenderStringToString("before{{foo}}after", new { foo = "FOO" });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_handles_two_variables_correctly()
        {
            var result = Template.RenderStringToString("before{{foo}}inside{{bar}}after", new { foo = "FOO", bar = "BAR" });
            Assert.AreEqual("beforeFOOinsideBARafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_false()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = false });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_true()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = true });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_null()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = (string)null });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_null()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = "bar" });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_empty_collections()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new int[] { } });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_empty_collections()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new [] { 1 } });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_empty_collections_for_each_item_in_the_collection()
        {
            var result = Template.RenderStringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new [] { 1, 2, 3 } });
            Assert.AreEqual("beforeFOOFOOFOOafter", result);
        }

        [Test]
        public void It_changes_the_context_for_each_item_in_the_collection()
        {
            var result = Template.RenderStringToString("before{{#foo}}{{.}}{{/foo}}after", new { foo = new [] { 1, 2, 3 } });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_lets_you_reference_properties_of_items_in_the_collection()
        {
            var result = Template.RenderStringToString(
                "before{{#foo}}{{bar}}{{/foo}}after",
                new { foo = new [] { new { bar = 1 }, new { bar = 2 }, new { bar = 3 } } });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_looks_up_the_stack_for_properties()
        {
            var result = Template.RenderStringToString(
                "{{#foo}}{{bar}}{{/foo}}",
                new { foo = new { }, bar = "baz" });
            Assert.AreEqual("baz", result);
        }
    }
}
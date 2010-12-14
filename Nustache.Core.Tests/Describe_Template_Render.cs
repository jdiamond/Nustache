using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Template_Render
    {
        [Test]
        public void It_renders_empty_strings_as_empty_strings()
        {
            var result = Render.StringToString("", null);
            Assert.AreEqual("", result);
        }

        [Test]
        public void It_replaces_undefined_variables_with_empty_strings_when_there_is_no_data()
        {
            var result = Render.StringToString("before{{foo}}after", null);
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_replaces_undefined_variables_with_empty_strings_when_there_is_data()
        {
            var result = Render.StringToString("before{{foo}}after", new { bar = "baz" });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_replaces_defined_variables_with_values()
        {
            var result = Render.StringToString("before{{foo}}after", new { foo = "FOO" });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_handles_two_variables_correctly()
        {
            var result = Render.StringToString("before{{foo}}inside{{bar}}after", new { foo = "FOO", bar = "BAR" });
            Assert.AreEqual("beforeFOOinsideBARafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_false()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = false });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_true()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = true });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_null()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = (string)null });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_null()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = "bar" });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_empty_collections()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new int[] { } });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_empty_collections()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new [] { 1 } });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_empty_collections_for_each_item_in_the_collection()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new [] { 1, 2, 3 } });
            Assert.AreEqual("beforeFOOFOOFOOafter", result);
        }

        [Test]
        public void It_changes_the_context_for_each_item_in_the_collection()
        {
            var result = Render.StringToString("before{{#foo}}{{.}}{{/foo}}after", new { foo = new [] { 1, 2, 3 } });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_lets_you_reference_properties_of_items_in_the_collection()
        {
            var result = Render.StringToString(
                "before{{#foo}}{{bar}}{{/foo}}after",
                new { foo = new [] { new { bar = 1 }, new { bar = 2 }, new { bar = 3 } } });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_looks_up_the_stack_for_properties()
        {
            var result = Render.StringToString(
                "{{#foo}}{{bar}}{{/foo}}",
                new { foo = new { /* no bar here */ }, bar = "baz" });
            Assert.AreEqual("baz", result);
        }

        [Test]
        public void It_pushes_and_pops_contexts_correctly()
        {
            var result = Render.StringToString(
                "{{bar}}{{#foo}}{{bar}}{{/foo}}{{bar}}",
                new { foo = new { bar = "quux" }, bar = "baz" });
            Assert.AreEqual("bazquuxbaz", result);
        }

        [Test]
        public void It_can_include_templates()
        {
            var fooTemplate = new Template();
            fooTemplate.Load(new StringReader("FOO"));

            var result = Render.StringToString(
                "before{{>foo}}after", null, name => fooTemplate);

            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_throws_to_prevent_infinite_template_recursion()
        {
            var fooTemplate = new Template();
            fooTemplate.Load(new StringReader("{{>foo}}"));

            Assert.Throws<NustacheException>(
                () => Render.StringToString(
                    "before{{>foo}}after", null, name => fooTemplate));
        }

        [Test]
        public void It_can_include_templates_defined_in_templates()
        {
            var result = Render.StringToString(
                "{{<foo}}FOO{{/foo}}before{{>foo}}after", null);

            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_can_include_templates_defined_in_outer_templates()
        {
            var result = Render.StringToString(
                "{{<foo}}OUTSIDE{{/foo}}before{{#bar}}{{>foo}}{{/bar}}after",
                new { bar = "baz" });

            Assert.AreEqual("beforeOUTSIDEafter", result);
        }

        [Test]
        public void It_allows_templates_to_be_overridden_in_sections()
        {
            var result = Render.StringToString(
                "{{<foo}}OUTSIDE{{/foo}}before{{#bar}}{{<foo}}INSIDE{{/foo}}{{>foo}}{{/bar}}after",
                new { bar = "baz" });

            Assert.AreEqual("beforeINSIDEafter", result);
        }
    }
}
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Scanner
    {
        [Test]
        public void It_returns_no_parts_for_the_empty_string()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("");

            parts.IsEmpty();
        }

        [Test]
        public void It_scans_literal_text()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("foo");

            parts.IsEqualTo(new LiteralText("foo"));
        }

        [Test]
        public void It_scans_variable_references()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{foo}}after");

            parts.IsEqualTo(new LiteralText("before"),
                            new VariableReference("foo"),
                            new LiteralText("after"));
        }

        [Test]
        public void It_allows_dots_in_variable_references()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{foo.bar}}after");

            parts.IsEqualTo(new LiteralText("before"),
                            new VariableReference("foo.bar"),
                            new LiteralText("after"));
        }

        [Test]
        public void It_allows_variable_references_to_be_surrounded_by_spaces()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{ foo }}after");

            parts.IsEqualTo(new LiteralText("before"),
                            new VariableReference("foo"),
                            new LiteralText("after"));
        }

        [Test]
        public void It_allows_unencoded_variable_references_to_be_surrounded_by_spaces()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{{ foo }}}after");

            parts.IsEqualTo(new LiteralText("before"),
                            new VariableReference("foo"),
                            new LiteralText("after"));
        }

        [Test]
        public void It_scans_sections()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("{{#foo}}inside{{/foo}}");

            parts.IsEqualTo(new Block("foo"),
                            new LiteralText("inside"),
                            new EndSection("foo"));
        }

        [Test]
        public void It_allows_dots_in_sections()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("{{#foo.bar}}inside{{/foo.bar}}");

            parts.IsEqualTo(new Block("foo.bar"),
                            new LiteralText("inside"),
                            new EndSection("foo.bar"));
        }

        [Test]
        public void It_allows_funny_whitespace_in_section_markers()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("{{ #foo }}inside{{ /foo }}");

            parts.IsEqualTo(new Block("foo"),
                            new LiteralText("inside"),
                            new EndSection("foo"));
        }

        [Test]
        public void It_scans_template_definitions()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("{{<foo}}inside{{/foo}}");

            parts.IsEqualTo(new TemplateDefinition("foo"),
                            new LiteralText("inside"),
                            new EndSection("foo"));
        }

        [Test]
        public void It_scans_template_includes()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{>foo}}after");

            parts.IsEqualTo(new LiteralText("before"),
                            new TemplateInclude("foo"),
                            new LiteralText("after"));
        }

        [Test]
        public void It_skips_comments()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before{{!foo}}after");

            parts.IsEqualTo(new LiteralText("before"),
                            new LiteralText("after"));
        }

        [Test]
        public void It_strips_out_new_lines_after_non_variable_markers()
        {
            var scanner = new Scanner();

            var parts = scanner.Scan("before\r\n{{#foo}} \r\n{{/foo}}\t\r\n\r\nafter");

            parts.IsEqualTo(new LiteralText("before\r\n"),
                            new Block("foo"),
                            new EndSection("foo"),
                            new LiteralText("\r\nafter"));
        }
    }
}

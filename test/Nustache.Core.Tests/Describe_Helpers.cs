using System.Collections;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Helpers
    {
        [SetUp]
        public void BeforeEach()
        {
            Helpers.Clear();
        }

        [Test]
        public void It_can_register_global_helpers()
        {
            Helpers.Register("noop", (ctx, args, opts, fn, inverse) => fn(null));

            var result = Render.StringToString("{{#noop}}{{value}}{{/noop}}", new {value = 42});

            Assert.AreEqual("42", result);
        }

        [Test]
        public void It_passes_arguments_into_helpers()
        {
            Helpers.Register("link", (ctx, args, opts, fn, inverse) => ctx.Write(string.Format("<a href=\"{0}\">{1}</a>", args[1], args[0])));

            var result = Render.StringToString("{{link text url}}", new {text = "TEXT", url = "URL"});

            Assert.AreEqual("<a href=\"URL\">TEXT</a>", result);
        }

        [Test]
        public void It_passes_options_into_helpers()
        {
            Helpers.Register("link", (ctx, args, opts, fn, inverse) => ctx.Write(string.Format("<a href=\"{0}\">{1}</a>", opts["url"], opts["text"])));

            var result = Render.StringToString("{{link text=\"TEXT\" url=\"URL\"}}", new {});

            Assert.AreEqual("<a href=\"URL\">TEXT</a>", result);
        }

        [Test]
        public void It_passes_arguments_into_block_helpers()
        {
            Helpers.Register("list", (ctx, args, opts, fn, inverse) =>
            {
                ctx.Write("<ul>");

                foreach (var item in (IEnumerable)args[0])
                {
                    ctx.Write("<li>");
                    fn(item);
                    ctx.Write("</li>");
                }

                ctx.Write("</ul>");
            });

            var result = Render.StringToString("{{#list nav}}<a href=\"{{url}}\">{{text}}</a>{{/list}}", new
            {
                nav = new[]
                {
                    new {url = "URL1", text = "TEXT1"},
                    new {url = "URL2", text = "TEXT2"}
                }
            });

            Assert.AreEqual("<ul><li><a href=\"URL1\">TEXT1</a></li><li><a href=\"URL2\">TEXT2</a></li></ul>", result);
        }

        [Test]
        public void It_parses_quoted_arguments_as_literal_strings()
        {
            Helpers.Register("link", (ctx, args, opts, fn, inverse) => ctx.Write(string.Format("<a href=\"{0}\">{1}</a>", args[1], args[0])));

            var result = Render.StringToString("{{link \"TEXT\" \"URL\"}}", new {});

            Assert.AreEqual("<a href=\"URL\">TEXT</a>", result);
        }

        [Test]
        public void It_parses_quoted_options_as_literal_strings()
        {
            Helpers.Register("link", (ctx, args, opts, fn, inverse) => ctx.Write(string.Format("<a href=\"{0}\">{1}</a>", opts["url"], opts["text"])));

            var result = Render.StringToString("{{link text=\"TEXT\" url=\"URL\"}}", new { });

            Assert.AreEqual("<a href=\"URL\">TEXT</a>", result);
        }

        [Test]
        public void It_registers_each_by_default()
        {
            var result = Render.StringToString(
                "<ul>{{#each things}}<li>{{.}}</li>{{/each}}</ul>",
                new { things = new[] { "thing1", "thing2", "thing3" } });

            Assert.AreEqual("<ul><li>thing1</li><li>thing2</li><li>thing3</li></ul>", result);
        }

        [Test]
        public void It_registers_if_by_default()
        {
            var template = "{{#if error}}<div class=\"alert alert-danger\">{{error.message}}</div>{{else}}<p>No errors</p>{{/if}}";

            var result = Render.StringToString(
                template,
                new {error = new {message = "Connection closed"}});

            Assert.AreEqual("<div class=\"alert alert-danger\">Connection closed</div>", result);

            result = Render.StringToString(
                template,
                new {error = false});

            Assert.AreEqual("<p>No errors</p>", result);
        }

        [Test]
        public void It_registers_unless_by_default()
        {
            var template = "{{#unless error}}<p>No errors</p>{{else}}<div class=\"alert alert-danger\">{{error.message}}</div>{{/unless}}";

            var result = Render.StringToString(
                template,
                new {error = new {message = "Connection closed"}});

            Assert.AreEqual("<div class=\"alert alert-danger\">Connection closed</div>", result);

            result = Render.StringToString(
                template,
                new {error = false});

            Assert.AreEqual("<p>No errors</p>", result);
        }

        [Test]
        public void It_registers_with_by_default()
        {
            var result = Render.StringToString(
                "<div class=\"alert alert-danger\">{{#with error}}<p>{{message}}</p>{{/with}}</div>",
                new { error = new { message = "Connection closed" } });

            Assert.AreEqual("<div class=\"alert alert-danger\"><p>Connection closed</p></div>", result);
        }
    }
}

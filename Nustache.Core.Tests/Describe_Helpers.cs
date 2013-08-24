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
            Helpers.Register("noop", (ctx, args, opts, fn) => fn(ctx));

            var result = Render.StringToString("{{#noop}}{{value}}{{/noop}}", new {value = 42});

            Assert.AreEqual("42", result);
        }

        [Test]
        public void It_passes_arguments_into_helpers()
        {
            Helpers.Register("link", (ctx, args, opts, fn) => ctx.Write(string.Format("<a href=\"{0}\">{1}</a>", args[1], args[0])));

            var result = Render.StringToString("{{link text url}}", new { text = "TEXT", url = "URL" });

            Assert.AreEqual("<a href=\"URL\">TEXT</a>", result);
        }

        [Test]
        public void It_parses_quoted_arguments_as_literal_strings()
        {
            Helpers.Register("link", (ctx, args, opts, fn) => ctx.Write(string.Format("<a href=\"{0}\">{1}</a>", args[1], args[0])));

            var result = Render.StringToString("{{link \"TEXT\" \"URL\"}}", new {});

            Assert.AreEqual("<a href=\"URL\">TEXT</a>", result);
        }
    }
}

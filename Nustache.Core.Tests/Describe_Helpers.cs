using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Helpers
    {
        [Test]
        public void It_can_register_global_helpers()
        {
            Helpers.Register("noop", (ctx, args, opts, fn) => fn(ctx));

            var result = Render.StringToString("{{#noop}}{{value}}{{/noop}}", new { value = 42 });

            Assert.AreEqual("42", result);
        }
    }
}

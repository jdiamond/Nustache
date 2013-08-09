using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class RenderContext_Behaviour_Support
    {
        [Test, ExpectedException(ExpectedException = typeof(NustacheDataContextMissException),
            ExpectedMessage = "Path : . is undefined, RaiseExceptionOnDataContextMiss : true.")]
        public void It_throws_an_exception_when_array_is_null_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            Render.StringToString("before{{#.}}{{.}}{{/.}}after", null , new RenderContextBehaviour{ RaiseExceptionOnDataContextMiss = true});
        }

        [Test, ExpectedException(ExpectedException = typeof(NustacheEmptyStringException),
        ExpectedMessage = "Path : foo is an empty string, RaiseExceptionOnEmptyStringValue : true.")]
        public void It_throws_an_exception_when_the_data_is_an_empty_string_and_the_render_context_behaviour_throw_on_empty_string_is_true()
        {
            Render.StringToString("before{{foo}}after", new{ foo = string.Empty}, new RenderContextBehaviour { RaiseExceptionOnEmptyStringValue = true });
        }

        [Test, ExpectedException(ExpectedException = typeof(NustacheDataContextMissException),
        ExpectedMessage = "Path : foo is undefined, RaiseExceptionOnDataContextMiss : true.")]
        public void It_throws_an_exception_when_there_is_no_data_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            Render.StringToString("before{{foo}}after", null, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = true });
        }

        [Test, ExpectedException(ExpectedException = typeof(NustacheDataContextMissException),
            ExpectedMessage = "Path : foo.bar is undefined, RaiseExceptionOnDataContextMiss : true.")]
        public void It_throws_an_exception_when_there_is_no_nested_data_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            Render.StringToString("before{{foo.bar}}after", new { foo = new { } }, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = true });
        }

        [Test, ExpectedException(ExpectedException = typeof(NustacheEmptyStringException),
            ExpectedMessage = "Path : foo is an empty string, RaiseExceptionOnEmptyStringValue : true.")]
        public void It_throws_an_exception_when_section_is_mapped_to_empty_string_and_the_render_context_behaviour_throw_on_empty_string_is_true()
        {
            Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = string.Empty }, new RenderContextBehaviour { RaiseExceptionOnEmptyStringValue = true });
        }

        [Test, ExpectedException(ExpectedException = typeof(NustacheDataContextMissException),
            ExpectedMessage = "Path : foo is undefined, RaiseExceptionOnDataContextMiss : true.")]
        public void It_throws_an_exception_when_section_is_mapped_to_null_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = (string)null }, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = true });
        }

        [Test]
        public void It_does_not_throw_an_exception_when_there_is_no_data_and_the_render_context_behaviour_throw_on_miss_is_false()
        {
            var result = Render.StringToString("before{{foo}}after", null, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = false });
            Assert.AreEqual("beforeafter", result);
        }


    }
}
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class RenderContext_Behaviour_Support
    {
        [Test]
        public void It_throws_an_exception_when_array_is_null_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            NustacheDataContextMissException exception = Assert.Throws<NustacheDataContextMissException>(() =>
            {
                Render.StringToString("before{{#.}}{{.}}{{/.}}after", null , new RenderContextBehaviour{ RaiseExceptionOnDataContextMiss = true});
            });
            Assert.AreEqual("Path : . is undefined, RaiseExceptionOnDataContextMiss : true.", exception.Message);
        }

        [Test]
        public void It_throws_an_exception_when_the_data_is_an_empty_string_and_the_render_context_behaviour_throw_on_empty_string_is_true()
        {
            NustacheEmptyStringException exception = Assert.Throws<NustacheEmptyStringException>(() =>
            {
                Render.StringToString("before{{foo}}after", new{ foo = string.Empty}, new RenderContextBehaviour { RaiseExceptionOnEmptyStringValue = true });
            });
            Assert.AreEqual("Path : foo is an empty string, RaiseExceptionOnEmptyStringValue : true.", exception.Message);
        }

        [Test]
        public void It_throws_an_exception_when_there_is_no_data_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            NustacheDataContextMissException exception = Assert.Throws<NustacheDataContextMissException>(() =>
            {
                Render.StringToString("before{{foo}}after", null, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = true });
            });
            Assert.AreEqual("Path : foo is undefined, RaiseExceptionOnDataContextMiss : true.", exception.Message);
        }

        [Test]
        public void It_throws_an_exception_when_there_is_no_nested_data_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            NustacheDataContextMissException exception = Assert.Throws<NustacheDataContextMissException>(() =>
            {
                Render.StringToString("before{{foo.bar}}after", new { foo = new { } }, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = true });
            });
            Assert.AreEqual("Path : foo.bar is undefined, RaiseExceptionOnDataContextMiss : true.", exception.Message);
        }

        [Test]
        public void It_throws_an_exception_when_section_is_mapped_to_empty_string_and_the_render_context_behaviour_throw_on_empty_string_is_true()
        {
            NustacheEmptyStringException exception = Assert.Throws<NustacheEmptyStringException>(() =>
            {
                Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = string.Empty }, new RenderContextBehaviour { RaiseExceptionOnEmptyStringValue = true });
            });
            Assert.AreEqual("Path : foo is an empty string, RaiseExceptionOnEmptyStringValue : true.", exception.Message);
        }

        [Test]
        public void It_throws_an_exception_when_section_is_mapped_to_null_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            NustacheDataContextMissException exception = Assert.Throws<NustacheDataContextMissException>(() =>
            {
                Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = (string)null }, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = true });
            });
            Assert.AreEqual("Path : foo is undefined, RaiseExceptionOnDataContextMiss : true.", exception.Message);
        }

        [Test]
        public void It_does_not_throw_an_exception_when_there_is_no_data_and_the_render_context_behaviour_throw_on_miss_is_false()
        {
            var result = Render.StringToString("before{{foo}}after", null, new RenderContextBehaviour { RaiseExceptionOnDataContextMiss = false });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void Use_custom_encoder()
        {
            var result = Render.StringToString("before{{foo}}after", new {foo = string.Empty},
                new RenderContextBehaviour {HtmlEncoder = text => "middle"});
            Assert.AreEqual("beforemiddleafter", result);
        }
    }
}
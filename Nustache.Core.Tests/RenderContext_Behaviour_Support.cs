using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class RenderContext_Behaviour_Support
    {
        [Test, ExpectedException(ExpectedException = typeof(NustacheContextMissException),
        ExpectedMessage = "Path : foo is null or undefined, RaiseExceptionOnDataContextMiss : true.")]
        public void It_throws_an_exception_when_there_is_no_data_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            var result = Render.StringToString("before{{foo}}after", null, RenderContextBehaviour.GetRenderContextBehaviour(true));
            Assert.AreEqual("beforeafter", result);
        }

        [Test, ExpectedException(ExpectedException = typeof(NustacheContextMissException),
            ExpectedMessage = "Path : foo.bar is null or undefined, RaiseExceptionOnDataContextMiss : true.")]
        public void It_throws_an_exception_when_there_is_no_nested_data_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            var result = Render.StringToString("before{{foo.bar}}after", new { foo = new { }}, RenderContextBehaviour.GetRenderContextBehaviour(true));
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_does_not_throw_an_exception_when_there_is_no_data_and_the_render_context_behaviour_throw_on_miss_is_false()
        {
            var result = Render.StringToString("before{{foo}}after", null, RenderContextBehaviour.GetRenderContextBehaviour(false));
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_does_not_an_exception_when_section_is_mapped_to_null_and_the_render_context_behaviour_throw_on_miss_is_true()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = (string) null }, RenderContextBehaviour.GetRenderContextBehaviour(true));
            Assert.AreEqual("beforeafter", result);
        }
    }
}
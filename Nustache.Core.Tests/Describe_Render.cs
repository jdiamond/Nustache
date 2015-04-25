using System.IO;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Render : FileSystemTestFixture
    {
        [Test]
        public void It_can_render_strings_to_strings()
        {
            var output = Render.StringToString("{{foo}}", new { foo = "bar" });

            Assert.AreEqual("bar", output);
        }

        [Test]
        public void It_does_not_include_a_byte_order_mark_when_rendering_to_strings()
        {
            var output = Render.StringToString("{{foo}}", new { foo = "bar" });

            StringAssert.DoesNotStartWith(output, "\uFEFF");
        }

        [Test]
        public void It_can_render_strings_to_files()
        {
            var outputPath = CreateEmptyFile();

            Render.StringToFile("{{foo}}", new { foo = "bar" }, outputPath);

            Assert.AreEqual("bar", File.ReadAllText(outputPath));
        }

        [Test]
        public void It_can_render_files_to_strings()
        {
            var templatePath = CreateFile("{{foo}}");

            var output = Render.FileToString(templatePath, new { foo = "bar" });

            Assert.AreEqual("bar", output);
        }

        [Test]
        public void It_can_render_files_to_files()
        {
            var templatePath = CreateFile("{{foo}}");
            var outputPath = CreateEmptyFile();

            Render.FileToFile(templatePath, new { foo = "bar" }, outputPath);

            Assert.AreEqual("bar", File.ReadAllText(outputPath));
        }

        [Test]
        public void It_can_render_encoded_text()
        {
          var result = Render.StringToString("{{foo}}", new { foo = "<bar>" });

          Assert.AreEqual("&lt;bar&gt;", result);
        }

        [Test]
        public void It_can_render_unencoded_text()
        {
            var result = Render.StringToString("{{{foo}}}", new { foo = "<bar>" });

            Assert.AreEqual("<bar>", result);
        }

        [Test]
        public void It_can_use_an_ampersand_to_render_unencoded_text()
        {
            var result = Render.StringToString("{{&foo}}", new { foo = "<bar>" });

            Assert.AreEqual("<bar>", result);
        }
    }
}
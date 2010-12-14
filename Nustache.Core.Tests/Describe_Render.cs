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
            var template = CreateFile("{{foo}}");

            var output = Render.FileToString(template, new { foo = "bar" });

            Assert.AreEqual("bar", output);
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
    }
}
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class Describe_Render
    {
        [Test]
        public void It_can_render_strings_to_strings()
        {
            var template = GetTemplatePath("{{foo}}");

            var output = Render.FileToString(template, new { foo = "bar" });

            Assert.AreEqual("bar", output);
        }

        [Test]
        public void It_can_render_strings_to_files()
        {
            var outputPath = GetOutputPath();

            Render.StringToFile("{{foo}}", new { foo = "bar" }, outputPath);

            Assert.AreEqual("bar", File.ReadAllText(outputPath));
        }

        [Test]
        public void It_can_render_files_to_strings()
        {
            var templatePath = GetTemplatePath("{{foo}}");

            var output = Render.FileToString(templatePath, new { foo = "bar" });

            Assert.AreEqual("bar", output);
        }

        [Test]
        public void It_can_render_files_to_files()
        {
            var templatePath = GetTemplatePath("{{foo}}");
            var outputPath = GetOutputPath();

            Render.FileToFile(templatePath, new { foo = "bar" }, outputPath);

            Assert.AreEqual("bar", File.ReadAllText(outputPath));
        }

        private List<string> _filesToDelete;

        private string GetTemplatePath(string template)
        {
            string path = Path.GetTempFileName();
            _filesToDelete.Add(path);
            File.WriteAllText(path, template);
            return path;
        }

        private string GetOutputPath()
        {
            string path = Path.GetTempFileName();
            _filesToDelete.Add(path);
            return path;
        }

        [SetUp]
        public void BeforeEach()
        {
            _filesToDelete = new List<string>();
        }

        [TearDown]
        public void AfterEach()
        {
            _filesToDelete.ForEach(File.Delete);
        }
    }
}
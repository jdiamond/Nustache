using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_FileSystemTemplateLocator : FileSystemTestFixture
    {
        [Test]
        public void It_locates_templates_with_the_specified_extension_in_the_specified_directory()
        {
            string path = CreateFile("foo");
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            var locator = new FileSystemTemplateLocator(ext, dir);

            var template = locator.GetTemplate(name);

            template.Parts.Single().IsEqualTo(new LiteralText("foo"));
        }

        [Test]
        public void It_returns_null_when_the_template_doesnt_exist()
        {
            string path = CreateFile("foo");
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            var locator = new FileSystemTemplateLocator(ext, dir);

            var template = locator.GetTemplate(name + "x");

            Assert.That(template, Is.Null);
        }
    }
}
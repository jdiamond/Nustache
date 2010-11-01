using System.IO;

namespace Nustache.Core
{
    public class FileSystemTemplateLocator
    {
        private readonly string _extension;
        private readonly string _directory;

        public FileSystemTemplateLocator(string extension, string directory)
        {
            _extension = extension;
            _directory = directory;
        }

        public Template GetTemplate(string name)
        {
            string path = Path.Combine(_directory, name + _extension);

            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                var reader = new StringReader(text);

                var template = new Template();
                template.Load(reader);

                return template;
            }

            return null;
        }
    }
}
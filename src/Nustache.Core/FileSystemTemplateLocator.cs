using System.IO;

namespace Nustache.Core
{
    public class FileSystemTemplateLocator
    {
        private readonly string _extension;
        private readonly string[] _directories;

        public FileSystemTemplateLocator(string extension, params string[] directories)
        {
            _extension = extension;
            _directories = directories;
        }

        public Template GetTemplate(string name)
        {
            foreach (var directory in _directories)
            {
                var path = Path.Combine(directory, name + _extension);

                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    var reader = new StringReader(text);
                    var template = new Template();
                    template.Load(reader);

                    return template;
                }
            }

            return null;
        }
    }
}
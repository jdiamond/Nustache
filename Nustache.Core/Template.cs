using System.Collections.Generic;
using System.IO;

namespace Nustache.Core
{
    public class Template
    {
        private IEnumerable<Part> _parts;

        public void Load(TextReader reader)
        {
            string template = reader.ReadToEnd();

            var scanner = new Scanner();
            var parser = new Parser();

            _parts = parser.Parse(scanner.Scan(template));
        }

        public void Render(object data, TextWriter writer)
        {
            var context = new RenderContext(data, writer);

            foreach (var part in _parts)
            {
                part.Render(context);
            }

            writer.Flush();
        }
   }
}
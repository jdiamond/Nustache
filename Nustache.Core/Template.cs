using System;
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

        public static string RenderStringToString(string template, object data)
        {
            var reader = new StringReader(template);
            var writer = new StringWriter();
            Render(reader, data, writer);
            return writer.GetStringBuilder().ToString();
        }

        public static string RenderFileToString(string templatePath, object data)
        {
            throw new NotImplementedException();
        }

        public static void RenderStringToFile(string template, object data, string outputPath)
        {
            throw new NotImplementedException();
        }

        public static void RenderFileToFile(string templatePath, object data, string outputPath)
        {
            throw new NotImplementedException();
        }

        public static void Render(TextReader reader, object data, TextWriter writer)
        {
            var template = new Template();
            template.Load(reader);
            template.Render(data, writer);
        }
   }
}
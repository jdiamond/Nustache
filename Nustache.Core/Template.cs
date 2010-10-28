using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nustache.Core
{
    public class Template
    {
        private IEnumerable<Part> _parts;

        /// <summary>
        /// Loads the template.
        /// </summary>
        /// <param name="reader">The object to read the template from.</param>
        /// <remarks>
        /// The <paramref name="reader" /> is read until it ends, but is not
        /// closed or disposed.
        /// </remarks>
        /// <exception cref="NustacheException">
        /// Thrown when the template contains a syntax error.
        /// </exception>
        public void Load(TextReader reader)
        {
            string template = reader.ReadToEnd();

            var scanner = new Scanner();
            var parser = new Parser();

            _parts = parser.Parse(scanner.Scan(template)).ToArray();

            // ToArray() forces the iterator to evaluate. I want exceptions
            // thrown by the scanner or parser to happen during Load and
            // not during Render.
        }

        /// <summary>
        /// Renders the template.
        /// </summary>
        /// <param name="data">The data to use to render the template.</param>
        /// <param name="writer">The object to write the output to.</param>
        /// <param name="templateLocator">The delegate to use to locate templates for inclusion.</param>
        /// <remarks>
        /// The <paramref name="writer" /> is flushed, but not closed or disposed.
        /// </remarks>
        public void Render(object data, TextWriter writer, Func<string, Template> templateLocator)
        {
            var context = new RenderContext(data, writer, templateLocator);

            Render(context);

            writer.Flush();
        }

        public void Render(RenderContext context)
        {
            foreach (var part in _parts)
            {
                part.Render(context);
            }
        }
    }
}
using System.IO;

namespace Nustache.Core
{
    public class Template : Section
    {
        public string StartDelimiter { get; set; }
        public string EndDelimiter { get; set; }

        public Template()
            : this("#template") // I'm not happy about this fake name.
        {
        }

        public Template(string name)
            : this(name, "{{", "}}")
        {
        }

        public Template(string name, string startDelimiter, string endDelimiter)
            : base(name)
        {
            StartDelimiter = startDelimiter;
            EndDelimiter = endDelimiter;
        }

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

            var scanner = new Scanner(StartDelimiter, EndDelimiter);
            var parser = new Parser();
            parser.Parse(this, scanner.Scan(template));
            // After load get the last state of the delimiters to save in the context.
            StartDelimiter = scanner.StartDelimiter;
            EndDelimiter = scanner.EndDelimiter;
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
        public void Render(object data, TextWriter writer, TemplateLocator templateLocator)
        {
            Render(data, writer, templateLocator, RenderContextBehaviour.GetDefaultRenderContextBehaviour());
        }

        public void Render(object data, TextWriter writer, TemplateLocator templateLocator, RenderContextBehaviour renderContextBehaviour)
        {
            var context = new RenderContext(this, data, writer, templateLocator, renderContextBehaviour);
            context.ActiveStartDelimiter = StartDelimiter;
            context.ActiveEndDelimiter = EndDelimiter;

            Render(context);

            writer.Flush();
        }
    }
}
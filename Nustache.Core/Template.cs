using System.IO;

namespace Nustache.Core
{
    public static class Template
    {
        public static string Render(string template, object data)
        {
            var writer = new StringWriter();

            var context = new DefaultRenderContext(writer, data);

            var scanner = new Scanner();
            var parser = new Parser();

            foreach (var part in parser.Parse(scanner.Scan(template)))
            {
                part.Render(context);
            }

            writer.Flush();

            return writer.GetStringBuilder().ToString();
        }
   }
}
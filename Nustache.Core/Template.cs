using System.IO;

namespace Nustache.Core
{
    public static class Template
    {
        public static string Render(string template, object data)
        {
            var writer = new StringWriter();
            var context = data as IContext ?? new DefaultContext(data);

            var scanner = new Scanner();
            var parser = new Parser();

            foreach (var part in parser.Parse(scanner.Scan(template)))
            {
                part.Render(writer, context);
            }

            writer.Flush();

            return writer.GetStringBuilder().ToString();
        }
   }
}
using System;
using System.IO;

namespace Nustache.Core
{
    public static class Render
    {
        public static string StringToString(string template, object data)
        {
            var reader = new StringReader(template);
            var writer = new StringWriter();
            Template(reader, data, writer);
            return writer.GetStringBuilder().ToString();
        }

        public static string FileToString(string templatePath, object data)
        {
            throw new NotImplementedException();
        }

        public static void StringToFile(string template, object data, string outputPath)
        {
            throw new NotImplementedException();
        }

        public static void FileToFile(string templatePath, object data, string outputPath)
        {
            throw new NotImplementedException();
        }

        public static void Template(TextReader reader, object data, TextWriter writer)
        {
            var template = new Template();
            template.Load(reader);
            template.Render(data, writer);
        }
    }
}
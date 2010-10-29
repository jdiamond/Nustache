using System;
using System.IO;

namespace Nustache.Core
{
    public static class Render
    {
        public static string StringToString(string template, object data)
        {
            return StringToString(template, data, null);
        }

        public static string StringToString(string template, object data, Func<string, Template> templateLocator)
        {
            var reader = new StringReader(template);
            var writer = new StringWriter();
            Template(reader, data, writer, templateLocator);
            return writer.GetStringBuilder().ToString();
        }

        public static string FileToString(string templatePath, object data)
        {
            var template = File.ReadAllText(templatePath);
            return StringToString(template, data);
        }

        public static void StringToFile(string template, object data, string outputPath)
        {
            var reader = new StringReader(template);
            using (var writer = new StreamWriter(File.OpenWrite(outputPath)))
            {
                Template(reader, data, writer, null);
            }
        }

        public static void FileToFile(string templatePath, object data, string outputPath)
        {
            var reader = new StringReader(File.ReadAllText(templatePath));
            using (var writer = new StreamWriter(File.OpenWrite(outputPath)))
            {
                Template(reader, data, writer, null);
            }
        }

        public static void Template(TextReader reader, object data, TextWriter writer, Func<string, Template> templateLocator)
        {
            var template = new Template();
            template.Load(reader);
            template.Render(data, writer, templateLocator);
        }
    }
}
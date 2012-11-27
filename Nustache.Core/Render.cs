using System.IO;

namespace Nustache.Core
{
    public static class Render
    {
        public static void FileToFile(string templatePath, object data, string outputPath)
        {
            FileToFile(templatePath, data, outputPath, null);
        }

        public static void FileToFile(string templatePath, object data, string outputPath, Options options)
        {
            var reader = new StringReader(File.ReadAllText(templatePath));
            FileSystemTemplateLocator templateLocator = GetTemplateLocator(templatePath);
            using (StreamWriter writer = File.CreateText(outputPath))
            {
                Template(reader, data, writer, templateLocator.GetTemplate, options);
            }
        }

        public static string FileToString(string templatePath, object data)
        {
            string template = File.ReadAllText(templatePath);
            FileSystemTemplateLocator templateLocator = GetTemplateLocator(templatePath);
            return StringToString(template, data, templateLocator.GetTemplate);
        }

        public static void StringToFile(string template, object data, string outputPath)
        {
            StringToFile(template, data, outputPath, null, null);
        }

        public static void StringToFile(string template, object data, string outputPath, Options options)
        {
            StringToFile(template, data, outputPath, null, options);
        }

        public static void StringToFile(
            string template, object data, string outputPath, TemplateLocator templateLocator)
        {
            StringToFile(template, data, outputPath, templateLocator, null);
        }

        public static void StringToFile(
            string template, object data, string outputPath, TemplateLocator templateLocator, Options options)
        {
            var reader = new StringReader(template);
            using (StreamWriter writer = File.CreateText(outputPath))
            {
                Template(reader, data, writer, templateLocator, options);
            }
        }

        public static string StringToString(string template, object data)
        {
            return StringToString(template, data, null, null);
        }

        public static string StringToString(string template, object data, Options options)
        {
            return StringToString(template, data, null, options);
        }

        public static string StringToString(string template, object data, TemplateLocator templateLocator)
        {
            return StringToString(template, data, templateLocator, null);
        }

        public static string StringToString(
            string template, object data, TemplateLocator templateLocator, Options options)
        {
            var reader = new StringReader(template);
            var writer = new StringWriter();
            Template(reader, data, writer, templateLocator, options);
            return writer.GetStringBuilder().ToString();
        }

        public static void Template(TextReader reader, object data, TextWriter writer)
        {
            Template(reader, data, writer, null, null);
        }

        public static void Template(
            TextReader reader, object data, TextWriter writer, TemplateLocator templateLocator, Options options)
        {
            if (options == null)
            {
                options = Options.Defaults();
            }

            var template = new Template();
            template.Load(reader);
            template.Render(data, writer, templateLocator, options);
        }

        private static FileSystemTemplateLocator GetTemplateLocator(string templatePath)
        {
            string dir = Path.GetDirectoryName(templatePath);
            string ext = Path.GetExtension(templatePath);
            return new FileSystemTemplateLocator(ext, dir);
        }
    }
}
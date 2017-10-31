using System.IO;

namespace Nustache.Core
{
    public static class Render
    {
        public static string StringToString(string template, object data, RenderContextBehaviour renderContextBehaviour = null)
        {
            var renderBehaviour = renderContextBehaviour ??
                                         RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            return StringToString(template, data, null, renderBehaviour);
        }

        public static string StringToString(string template, object data, TemplateLocator templateLocator, RenderContextBehaviour renderContextBehaviour = null)
        {
            var reader = new StringReader(template);
            var writer = new StringWriter();

            var renderBehaviour = renderContextBehaviour ??
                                         RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            Template(reader, data, writer, templateLocator, renderBehaviour);
            return writer.GetStringBuilder().ToString();
        }

        public static string FileToString(string templatePath, object data, RenderContextBehaviour renderContextBehaviour = null)
        {
            var template = File.ReadAllText(templatePath);
            var templateLocator = GetTemplateLocator(templatePath);

            var renderBehaviour = renderContextBehaviour ??
                                         RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            return StringToString(template, data, templateLocator.GetTemplate, renderBehaviour);
        }

        public static void StringToFile(string template, object data, string outputPath, RenderContextBehaviour renderContextBehaviour = null)
        {
            var renderBehaviour = renderContextBehaviour ??
                                         RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            StringToFile(template, data, outputPath, null, renderBehaviour);
        }

        public static void StringToFile(string template, object data, string outputPath, TemplateLocator templateLocator, RenderContextBehaviour renderContextBehaviour = null)
        {
            var reader = new StringReader(template);

            var renderBehaviour = renderContextBehaviour ??
                                        RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            using (var writer = File.CreateText(outputPath))
            {
                Template(reader, data, writer, templateLocator, renderBehaviour);
            }
        }

        public static void FileToFile(string templatePath, object data, string outputPath, RenderContextBehaviour renderContextBehaviour = null)
        {
            var reader = new StringReader(File.ReadAllText(templatePath));
            var templateLocator = GetTemplateLocator(templatePath);

            var renderBehaviour = renderContextBehaviour ??
                                        RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            using (var writer = File.CreateText(outputPath))
            {
                Template(reader, data, writer, templateLocator.GetTemplate, renderBehaviour);
            }
        }

        public static void Template(TextReader reader, object data, TextWriter writer, RenderContextBehaviour renderContextBehaviour = null)
        {
            var renderBehaviour = renderContextBehaviour ??
                                        RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            Template(reader, data, writer, null, renderBehaviour);
        }

        public static void Template(TextReader reader, object data, TextWriter writer, TemplateLocator templateLocator, RenderContextBehaviour renderContextBehaviour = null)
        {
            var template = new Template();
            template.Load(reader);

            var renderBehaviour = renderContextBehaviour ??
                                        RenderContextBehaviour.GetDefaultRenderContextBehaviour();

            template.Render(data, writer, templateLocator, renderBehaviour);
        }

        private static FileSystemTemplateLocator GetTemplateLocator(string templatePath)
        {
            string dir = Path.GetDirectoryName(templatePath);
            string ext = Path.GetExtension(templatePath);
            return new FileSystemTemplateLocator(ext, dir);
        }
    }
}
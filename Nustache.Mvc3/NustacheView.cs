using System.IO;
using System.Web.Mvc;
using Nustache.Core;

namespace Nustache.Mvc
{
    public class NustacheView : IView
    {
        private readonly ControllerContext _controllerContext;
        private readonly string _viewPath;
        private readonly string _masterPath;

        public NustacheView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            _controllerContext = controllerContext;
            _viewPath = viewPath;
            _masterPath = masterPath;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            if (!string.IsNullOrEmpty(_masterPath))
            {
                var masterTemplate = GetTemplate(_masterPath);

                masterTemplate.Render(viewContext.ViewData.Model ?? viewContext.ViewData, writer,
                    name =>
                        {
                            if (name == "Body")
                            {
                                return GetTemplate(_viewPath);
                            }

                            // TODO: Figure out how to render sections.
                            // Wouldn't those be defined in the view like in Razor?

                            return null;
                        });
            }
            else
            {
                var viewTemplate = GetTemplate(_viewPath);
                viewTemplate.Render(viewContext.ViewData.Model ?? viewContext.ViewData, writer, null);
            }
        }

        private Template GetTemplate(string path)
        {
            // TODO: Add caching?
            var templatePath = _controllerContext.HttpContext.Server.MapPath(path);
            var templateSource = File.ReadAllText(templatePath);
            var template = new Template();
            template.Load(new StringReader(templateSource));
            return template;
        }
    }
}